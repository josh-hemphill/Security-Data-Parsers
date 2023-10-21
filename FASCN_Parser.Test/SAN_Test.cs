using System.Security.Cryptography.X509Certificates;

namespace FASCN_Parser.Test;

public class SAN_Test {

	[Fact]
	public void Test1() {
		FASCN? x = null;
		try {
			x = new FASCN( LaunchSettings.GetCert() );
		}
		finally {
		}
		if (x is not null && x.personIdentifier.Length > 0) {
			Console.WriteLine( x.personIdentifier );
		}
	}
}
