namespace SubjectAlternativeName.Test;

public class SAN_Test {

	[Fact]
	public void Test1() {
		SAN z = new( LaunchSettings.GetCert() );
		Assert.NotNull( z?.First?.PrincipalName );
	}
}
