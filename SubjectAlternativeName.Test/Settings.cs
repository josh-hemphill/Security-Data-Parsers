using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SubjectAlternativeName.Test;
public static class LaunchSettings {

  public static readonly string? CERT_KIND;
  public static readonly string? CERT_STARTS;
  public static readonly string? CERT_PATH;
  public static readonly string? CERT_PASS;
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
		lCert = CERT_PASS is not null && CERT_PASS.Length > 0 ?
			new X509Certificate2( File.ReadAllBytes( CERT_PATH ), CERT_PASS ) :
			new X509Certificate2( File.ReadAllBytes( CERT_PATH ) );
	 }
	 return lCert is null ? throw new Exception( "No cert specified for parsing" ) : lCert;
  }
  static LaunchSettings() {
	 using StreamReader file = File.OpenText( "local.envSettings.json" );
	 JsonTextReader reader = new( file );
	 JObject jObject = JObject.Load( reader );

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
		  default:
			 break;
		}
	 }
  }

}
