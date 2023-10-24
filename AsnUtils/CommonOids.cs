namespace AsnUtils;

internal static class CommonOids {
	public static OidDescription PrincipalName => new() {
		Id = "1.3.6.1.4.1.311.20.2.3",
		FriendlyName = "User Principal Name (UPN)",
		ShortName = "PrincipalName",
		Abbreviation = "UPN",
	};
	public static OidDescription FASCN => new() {
		Id = "2.16.840.1.101.3.6.6",
		FriendlyName = "Federal Agency Smart Credential Number (FASC-N)",
		ShortName = "FASC-N",
		Abbreviation = "FASCN",
	};
	public static OidDescription SubjectAlternativeName => new() {
		Id = "2.5.29.17",
		FriendlyName = "Subject Alternative Name",
		ShortName = "SubjectAltName",
		Abbreviation = "SAN",
	};
}
