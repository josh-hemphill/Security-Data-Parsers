using TestUtils;
namespace SecurityDataParsers.FederalAgencySmartCredentialNumber.Test;

public class SAN_Test {

	[Fact]
	public void LoadsBasicCert() {
		FASCN? x = FASCN.FromCertificate( LaunchSettings.GetCert() );
		Assert.NotNull( x.personIdentifier );
	}
}
