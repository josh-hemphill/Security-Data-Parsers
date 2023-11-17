using System.Security.Cryptography.X509Certificates;
using static TestUtils.TestFiles.Cert;

namespace SecurityDataParsers.SubjectAlternativeName.Test;

public class CertTest {
	private static readonly X509Certificate2 CertBasic = new( CreateCustom(
		"CN=example.com",
		new[]
		{
			MakeNames.DnsName("example.com"),
			MakeNames.URI("urn:1234"),
			MakeNames.Email("MyPrincipalName@example.com"),
			MakeNames.DirectoryName("CN=Test"),
			MakeNames.IPAddress("192.168.0.1"),
			MakeNames.PrincipalName("MyPrincipalName@example.com"),
			MakeNames.FASCN("D22010DA010C2D00843C0D8360DA01084210843082201093EB")
		}
	).GetEncoded() );
	[Fact]
	public void BasicCertProperties() {
		SAN cert = new( CertBasic );
		Assert.Equal( "example.com", cert.First.DnsName?.Host );
		Assert.Equal( "urn:1234", cert.First.UniformResourceIdentifier?.AbsoluteUri );
		Assert.Equal( "MyPrincipalName@example.com", cert.First.Rfc822Name );
		Assert.Equal( "MyPrincipalName@example.com", cert.First.Email );
		Assert.Equal( "CN=Test", cert.First.DirectoryName?.ToString() );
		Assert.Equal( "192.168.0.1", cert.First.IPAddress?.ToString() );
		Assert.Equal( "MyPrincipalName@example.com", cert.First.PrincipalName );
		Assert.NotNull( cert.First.FASCN?.credentialNumber.ToString() );
	}


	private static readonly X509Certificate2 CertMultiParam = new( CreateCustom(
		"CN=example.com",
		new[]
		{
			MakeNames.DnsName("example.com"),
			MakeNames.DnsName("example2.com"),
			MakeNames.URI("urn:1234"),
			MakeNames.URI("urn:12345"),
			MakeNames.Email("MyPrincipalName@example.com"),
			MakeNames.Email("MyPrincipalName@example2.com"),
			MakeNames.DirectoryName("CN=Test"),
			MakeNames.DirectoryName("CN=Test2"),
			MakeNames.IPAddress("192.168.0.1"),
			MakeNames.IPAddress("192.168.0.2"),
			MakeNames.PrincipalName("MyPrincipalName@example.com"),
			MakeNames.PrincipalName("MyPrincipalName@example2.com"),
			MakeNames.FASCN("D22010DA010C2D00843C0D8360DA01084210843082201093EB"),
			MakeNames.FASCN("D22010DA010C2D00843C0D8360DA01084210843082201093EB")
		}
	).GetEncoded() );
	[Fact]
	public void MultiParamCertProperties() {
		SAN cert = new( CertMultiParam );
		Assert.Equal( "example.com", cert.First.DnsName?.Host );
		Assert.Equal( 2, cert.dnsNames.Count );
		Assert.Equal( "urn:1234", cert.First.UniformResourceIdentifier?.AbsoluteUri );
		Assert.Equal( 2, cert.uniformResourceIdentifiers.Count );
		Assert.Equal( "MyPrincipalName@example.com", cert.First.Rfc822Name );
		Assert.Equal( 2, cert.rfc822Names.Count );
		Assert.Equal( "CN=Test", cert.First.DirectoryName?.ToString() );
		Assert.Equal( 2, cert.directoryNames.Count );
		Assert.Equal( "192.168.0.1", cert.First.IPAddress?.ToString() );
		Assert.Equal( 2, cert.iPAddresses.Count );
		Assert.Equal( "MyPrincipalName@example.com", cert.First.PrincipalName );
		Assert.Equal( 2, cert.otherNames.principalNames.Count );
		Assert.NotNull( cert.First.FASCN?.credentialNumber.ToString() );
		Assert.Equal( 2, cert.otherNames.FASCNs.Count );
	}
}
