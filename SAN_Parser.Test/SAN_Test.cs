namespace SAN_Parser.Test;

public class SAN_Test {

	[Fact]
	public void Test1() {
		SAN z;
		try {
			z = new SAN( LaunchSettings.GetCert() );
			if (z.principalName is not null) {
				string realm = z.principalName.Realm;
				Console.WriteLine( realm );
			}
			Console.WriteLine( z.fASCN?.agencyCode.Digits );
		}
		finally {

		}
	}
}
