using TestUtils;
namespace SecurityDataParsers.SubjectAlternativeName.Test;

public class SAN_Test {

	[Fact]
	public void LoadsBasicCert() {
		SAN z = new( LaunchSettings.GetCert() );
		Assert.NotNull( z?.First?.PrincipalName );
	}
	[Fact]
	public void LoadsCertExt() {
		byte[] ext = LaunchSettings.GetCert().Extensions.Single( v => v.Oid?.Value == "2.5.29.17" ).RawData;
		System.Security.Cryptography.X509Certificates.X509SubjectAlternativeNameExtension x = new( ext );
		SAN z = new( x );
		Assert.NotNull( z?.First?.PrincipalName );
		z = new( ext );
		Assert.NotNull( z?.First?.PrincipalName );
	}
}
