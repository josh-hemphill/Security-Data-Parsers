using System.Formats.Asn1;

namespace SAN_Parser;

public static class AsnStructures {
	public readonly struct OIdDescription {
		public required string Id { get; init; }
		public required string FriendlyName { get; init; }
		public required string ShortName { get; init; }
		public required string Abbreviation { get; init; }
	}
	public static class AsnTags {
		public static readonly Asn1Tag Uri = new( TagClass.ContextSpecific, tagValue: 6, isConstructed: false );
		public static class OtherName {
			public static readonly Asn1Tag Tag = new( TagClass.ContextSpecific, tagValue: 0, isConstructed: true );
			public static readonly Asn1Tag ObjID = new( TagClass.Universal, tagValue: 6, isConstructed: false );
			public static readonly Asn1Tag Obj = new( TagClass.ContextSpecific, tagValue: 0, isConstructed: true );
			public static class OIds {
				public static readonly Asn1Tag Tag = new( TagClass.Universal, tagValue: 3, isConstructed: false );
				public static readonly OIdDescription principalName = new() {
					Id = "1.3.6.1.4.1.311.20.2.3",
					FriendlyName = "User Principal Name (UPN)",
					ShortName = "PrincipalName",
					Abbreviation = "UPN",
				};
				public static readonly OIdDescription FASCN = new() {
					Id = "2.16.840.1.101.3.6.6",
					FriendlyName = "Federal Agency Smart Credential Number (FASC-N)",
					ShortName = "FASC-N",
					Abbreviation = "FASCN",
				};
			}
		}
	}
}
