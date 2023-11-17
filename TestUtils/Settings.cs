using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
			lCert = new X509Certificate2( TestFiles.Cert.TestCert.GetEncoded() );
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
