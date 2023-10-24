using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509;

namespace TestUtils;
public static class LaunchSettings {

	public static readonly string? CERT_KIND = Environment.GetEnvironmentVariable( "SAN_PARSER_TEST_CERT_KIND" );
	public static readonly string? CERT_STARTS = Environment.GetEnvironmentVariable( "SAN_PARSER_TEST_CERT_STARTS" );
	public static readonly string? CERT_PATH = Environment.GetEnvironmentVariable( "SAN_PARSER_TEST_CERT_PATH" );
	public static readonly string? CERT_PASS = Environment.GetEnvironmentVariable( "SAN_PARSER_TEST_CERT_PASS" );
	public static readonly string? KEY_PATH = Environment.GetEnvironmentVariable( "SAN_PARSER_TEST_KEY_PATH" );
	private static X509Certificate2? lCert = null;

	public static X509Certificate2 GetCert() {
		if (lCert is not null) return lCert;
		if (CERT_KIND == "STORE" && CERT_STARTS?.Length > 0) {
			lCert = Array.Find(
				new X509Store( StoreName.My, StoreLocation.CurrentUser, OpenFlags.ReadOnly ).Certificates.ToArray(),
				v => v.Thumbprint.StartsWith( CERT_STARTS )
			);
		} else if (
			CERT_KIND == "FILE" &&
			CERT_PATH?.Length > 0
		) {
			byte[] certFile = File.ReadAllBytes( CERT_PATH );
			lCert = KEY_PATH?.Length > 0
			  ? CERT_PASS is not null && CERT_PASS.Length > 0 ?
					X509Certificate2.CreateFromEncryptedPemFile( certFile.ToString() ?? "", CERT_PASS, KEY_PATH ) :
					X509Certificate2.CreateFromPemFile( certFile.ToString() ?? "", KEY_PATH )
			  : CERT_PASS is not null && CERT_PASS.Length > 0 ?
					new X509Certificate2( certFile, CERT_PASS ) :
					new X509Certificate2( certFile );
		} else if (CERT_KIND == "DEFAULT_TEST") {
			lCert = new X509Certificate2( EnsureFiles.Cert.TestCert.GetEncoded() );
		}
		return lCert is null ? throw new Exception( "No cert specified for parsing" ) : lCert;
	}
	static LaunchSettings() {
		JObject jObject;
		try {
			using StreamReader file = File.OpenText( "local.envSettings.json" );
			JsonTextReader reader = new( file );
			jObject = JObject.Load( reader );
		}
		catch (Exception) {
			Console.WriteLine( "could not open file" );
			return;
		}

		List<JProperty> variables = jObject
				 .GetValue( "environmentVariables" )
				 ?.Children<JProperty>()
				 .ToList() ?? new List<JProperty>();

		foreach (JProperty? variable in variables) {
			Environment.SetEnvironmentVariable( variable.Name, variable.Value.ToString() );
			switch (variable.Name) {
				case "SAN_PARSER_TEST_CERT_KIND":
					CERT_KIND = variable.Value.ToString();
					break;
				case "SAN_PARSER_TEST_CERT_STARTS":
					CERT_STARTS = variable.Value.ToString();
					break;
				case "SAN_PARSER_TEST_CERT_PATH":
					CERT_PATH = variable.Value.ToString();
					break;
				case "SAN_PARSER_TEST_CERT_PASS":
					CERT_PASS = variable.Value.ToString();
					break;
				case "SAN_PARSER_TEST_KEY_PATH":
					KEY_PATH = variable.Value.ToString();
					break;
				default:
					break;
			}
		}
	}

}

public static class EnsureFiles {
	public static class Cert {
		private static Org.BouncyCastle.X509.X509Certificate? _TestCert;
		public static Org.BouncyCastle.X509.X509Certificate TestCert => _TestCert ?? CreateCert();
		private static readonly SecureRandom secureRandom = new();

		private static AsymmetricCipherKeyPair GenerateRsaKeyPair( int length ) {
			KeyGenerationParameters keygenParam = new( secureRandom, length );

			RsaKeyPairGenerator keyGenerator = new();
			keyGenerator.Init( keygenParam );
			return keyGenerator.GenerateKeyPair();
		}

		private static Org.BouncyCastle.X509.X509Certificate GenerateCertificate(
			  X509Name issuer, X509Name subject,
			  AsymmetricKeyParameter issuerPrivate,
			  AsymmetricKeyParameter subjectPublic ) {
			ISignatureFactory signatureFactory = new Asn1SignatureFactory(
					 PkcsObjectIdentifiers.Sha256WithRsaEncryption.ToString(),
					 issuerPrivate );
			X509V3CertificateGenerator certGenerator = new();
			certGenerator.SetIssuerDN( issuer );
			certGenerator.SetSubjectDN( subject );
			certGenerator.SetSerialNumber( BigInteger.ValueOf( 1 ) );
			certGenerator.SetNotAfter( DateTime.UtcNow.AddHours( 1 ) );
			certGenerator.SetNotBefore( DateTime.UtcNow );
			certGenerator.SetPublicKey( subjectPublic );

			static DerTaggedObject getOtherName( string id, string val, bool isFASCN = false ) {
				return new DerTaggedObject( false, 0, new DerSequence( new Asn1EncodableVector() {
					new DerObjectIdentifier( id ),
					isFASCN
						? new DerTaggedObject( true, GeneralName.OtherName, new DerOctetString( Hex.Decode(val) ) )
						: new DerTaggedObject( true, GeneralName.OtherName, new DerUtf8String( val ) )
				} ) );
			}

			Asn1Encodable[] subjectAlternativeNames = new[]
				{
					new GeneralName(GeneralName.DnsName, "server.localhost"),
					new GeneralName(GeneralName.UniformResourceIdentifier, "urn:1234"),
					new GeneralName(GeneralName.OtherName, getOtherName( "1.3.6.1.4.1.311.20.2.3", "MyPrincipalName@company" )),
					new GeneralName(GeneralName.OtherName, getOtherName( "2.16.840.1.101.3.6.6", "D22010DA010C2D00843C0D8360DA01084210843082201093EB", true ))
				};
			DerSequence subjectAlternativeNamesExtension = new( subjectAlternativeNames );
			certGenerator.AddExtension(
				 X509Extensions.SubjectAlternativeName.Id, false, subjectAlternativeNamesExtension );

			return certGenerator.Generate( signatureFactory );
		}

		private static bool ValidateSelfSignedCert( Org.BouncyCastle.X509.X509Certificate cert, ICipherParameters pubKey ) {
			cert.CheckValidity( DateTime.UtcNow );
			byte[] tbsCert = cert.GetTbsCertificate();
			byte[] sig = cert.GetSignature();

			ISigner signer = SignerUtilities.GetSigner( cert.SigAlgName );
			signer.Init( false, pubKey );
			signer.BlockUpdate( tbsCert, 0, tbsCert.Length );
			return signer.VerifySignature( sig );
		}

		private static Org.BouncyCastle.X509.X509Certificate CreateCert() {
			X509Name caName = new( "CN=TestCA" );
			X509Name eeName = new( "CN=TestEE" );
			AsymmetricCipherKeyPair caKey = GenerateRsaKeyPair( 2048 );
			AsymmetricCipherKeyPair eeKey = GenerateRsaKeyPair( 2048 );
			Org.BouncyCastle.X509.X509Certificate eeCert = GenerateCertificate( caName, eeName, caKey.Private, eeKey.Public );
			_TestCert = eeCert;
			return eeCert;
		}
	}
}
