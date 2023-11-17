using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;

namespace TestUtils;


public static class TestFiles {
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
		public static DerTaggedObject GetOtherName( string id, string val, bool isFASCN = false ) {
			return new DerTaggedObject( false, 0, new DerSequence( new Asn1EncodableVector() {
					new DerObjectIdentifier( id ),
					isFASCN
						? new DerTaggedObject( true, GeneralName.OtherName, new DerOctetString( Hex.Decode(val) ) )
						: new DerTaggedObject( true, GeneralName.OtherName, new DerUtf8String( val ) )
				} ) );
		}
		public static class MakeNames {
			public static GeneralName DnsName( string name = "server.localhost" ) => new( GeneralName.DnsName, name );
			public static GeneralName URI( string name = "urn:1234" ) => new( GeneralName.UniformResourceIdentifier, name );
			public static GeneralName Email( string name = "MyPrincipalName@company.com" ) => new( GeneralName.Rfc822Name, name );
			public static GeneralName DirectoryName( string name = "CN=Test" ) => new( GeneralName.DirectoryName, name );
			public static GeneralName IPAddress( string name = "192.168.0.1" ) => new( GeneralName.IPAddress, name );
			public static GeneralName PrincipalName( string name = "MyPrincipalName@company" ) =>
				new( GeneralName.OtherName, GetOtherName( "1.3.6.1.4.1.311.20.2.3", name ) );
			public static GeneralName FASCN( string name = "D22010DA010C2D00843C0D8360DA01084210843082201093EB" ) =>
				new( GeneralName.OtherName, GetOtherName( "2.16.840.1.101.3.6.6", name, true ) );
		}
		public static readonly Asn1Encodable[] DefaultSANs = new[] { MakeNames.DnsName(), MakeNames.URI(), MakeNames.PrincipalName(), MakeNames.FASCN() };

		private static Org.BouncyCastle.X509.X509Certificate GenerateCertificate(
			  X509Name issuer, X509Name subject,
			  AsymmetricKeyParameter issuerPrivate,
			  AsymmetricKeyParameter subjectPublic,
			  Asn1Encodable[]? sans = null
			) {
			ISignatureFactory signatureFactory = new Asn1SignatureFactory(
					 PkcsObjectIdentifiers.Sha256WithRsaEncryption.ToString(),
					 issuerPrivate );
			Org.BouncyCastle.X509.X509V3CertificateGenerator certGenerator = new();
			certGenerator.SetIssuerDN( issuer );
			certGenerator.SetSubjectDN( subject );
			certGenerator.SetSerialNumber( BigInteger.ValueOf( 1 ) );
			certGenerator.SetNotAfter( DateTime.UtcNow.AddHours( 1 ) );
			certGenerator.SetNotBefore( DateTime.UtcNow );
			certGenerator.SetPublicKey( subjectPublic );

			Asn1Encodable[] subjectAlternativeNames = sans?.Length > 0 ? sans : DefaultSANs;
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

		public static Org.BouncyCastle.X509.X509Certificate CreateCustom(
			string cn,
			Asn1Encodable[]? sans = null,
			string? caCN = null
		) {
			X509Name caName = new( caCN?.Length > 0 ? caCN : $"{cn};DN=CA" );
			X509Name eeName = new( cn );
			AsymmetricCipherKeyPair caKey = GenerateRsaKeyPair( 2048 );
			AsymmetricCipherKeyPair eeKey = GenerateRsaKeyPair( 2048 );
			Org.BouncyCastle.X509.X509Certificate eeCert = GenerateCertificate( caName, eeName, caKey.Private, eeKey.Public, sans );
			return eeCert;
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

