using TestUtils;
using SecurityDataParsers.FederalAgencySmartCredentialNumber;
namespace FederalAgencySmartCredentialNumber.Test;

public class SAN_Test {

	[Fact]
	public void Test1() {
		FASCN? x = FASCN.FromCertificate( LaunchSettings.GetCert() );
		Assert.NotNull( x.personIdentifier );
	}
}
