namespace FederalAgencySmartCredentialNumber.Test;

public class SAN_Test {

	[Fact]
	public void Test1() {
		FASCN? x;
		try {
			x = FASCN.FromCertificate( LaunchSettings.GetCert() );
		}
		finally {
		}
		if (x is not null && x.personIdentifier.Length > 0) {
			Console.WriteLine( x.personIdentifier );
		}
	}
}
