using System.Collections.ObjectModel;

namespace FASCN_Parser;

public static class Segments {
	public static readonly byte FlagLength = 1;
	public interface ISegment {
		public byte Length { get; }
	}
	public class FlagSegment : ISegment {
		public byte Length { get; } = FlagLength;
		public int MustEqual { get; init; }
	}
	public interface IValueSegment : ISegment {
		public byte[] Digits { get; set; }
		public abstract string FieldName { get; }
		public abstract string Abbreviation { get; }
	}
	public interface IEnumSegment {
		public string GetFriendlyName();
	}

	private static readonly Func<int, FlagSegment> IntToFlag = ( b ) => new() { MustEqual = b };

	public static readonly FlagSegment StartSentinel = IntToFlag( 0xB );
	public static readonly FlagSegment FieldSeparator = IntToFlag( 0xD );
	public static readonly FlagSegment EndSentinel = IntToFlag( 0xF );

	public class AgencyCode : IValueSegment, IEnumSegment {
		public byte Length { get; } = 4;
		public byte[] Digits { get; set; } = Array.Empty<byte>();
		public string FieldName { get; } = "Agency Code";
		public string Abbreviation { get; } = "AC";

		public string GetFriendlyName() {
			return this?.Digits != null ? NameMap.GetValueOrDefault( (Digits[0], Digits[1], Digits[2], Digits[3]), "Unknown" ) : "";
		}
		public static readonly ReadOnlyDictionary<(byte, byte, byte, byte), string> NameMap = new( new Dictionary<(byte, byte, byte, byte), string>() {
				{ (2,1,0,0), "Department of the Army" },
				{ (1,7,0,0), "Department of the Navy" },
				{ (1,7,2,7), "Department of the Navy – U.S. Marine Corps" },
				{ (5,7,0,0), "Department of the Air Force" },
				{ (9,7,0,0), "Department of Defense – Other Agencies" },
				{ (7,0,0,8), "U.S. Coast Guard" },
				{ (7,5,2,0), "U.S. Public Health Service" },
				{ (1,3,3,0), "National Oceanic and Atmospheric Administration" },
			} );
	};
	public class SystemCode : IValueSegment {
		public byte Length { get; } = 4;
		public byte[] Digits { get; set; } = Array.Empty<byte>();
		public string FieldName { get; } = "System Code";
		public string Abbreviation { get; } = "SC";
	};
	public class CredentialNumber : IValueSegment {
		public byte Length { get; } = 6;
		public byte[] Digits { get; set; } = Array.Empty<byte>();
		public string FieldName { get; } = "Credential Number";
		public string Abbreviation { get; } = "CN";
	};
	public class CredentialSeries : IValueSegment {
		public byte Length { get; } = 1;
		public byte[] Digits { get; set; } = Array.Empty<byte>();
		public string FieldName { get; } = "Credential Series";
		public string Abbreviation { get; } = "CS";
	};
	public class IndividualCredentialIssue : IValueSegment {
		public byte Length { get; } = 1;
		public byte[] Digits { get; set; } = Array.Empty<byte>();
		public string FieldName { get; } = "Individual Credential Issue";
		public string Abbreviation { get; } = "ICI";
	};
	public class PersonIdentifier : IValueSegment {
		public byte Length { get; } = 10;
		public byte[] Digits { get; set; } = Array.Empty<byte>();
		public string FieldName { get; } = "Person Identifier";
		public string Abbreviation { get; } = "PI";
	};
	public class OrganizationalCategory : IValueSegment, IEnumSegment {
		public byte Length { get; } = 1;
		public byte[] Digits { get; set; } = Array.Empty<byte>();
		public string FieldName { get; } = "Organizational Category";
		public string Abbreviation { get; } = "OC";

		public string GetFriendlyName() {
			return this?.Digits != null ? NameMap.GetValueOrDefault( Digits[0], "Unknown" ) : "";
		}

		public static readonly ReadOnlyDictionary<byte, string> NameMap = new( new Dictionary<byte, string>() {
				{  1, "Federal Government Agency" },
				{  2, "State Government Agency" },
				{  3, "Commercial Enterprise" },
				{  4, "Foreign Government" },
			} );
	};
	public class OrganizationIdentifier : IValueSegment {
		public byte Length { get; } = 4;
		public byte[] Digits { get; set; } = Array.Empty<byte>();
		public string FieldName { get; } = "IOrganization Identifier";
		public string Abbreviation { get; } = "OI";
	};
	public class PersonOrOrganizationAssociationCategory : IValueSegment, IEnumSegment {
		public byte Length { get; } = 1;
		public byte[] Digits { get; set; } = Array.Empty<byte>();
		public string FieldName { get; } = "Person/Organization Association Category";
		public string Abbreviation { get; } = "POA";

		public string GetFriendlyName() {
			return this?.Digits != null ? NameMap.GetValueOrDefault( Digits[0], "Unknown" ) : "";
		}

		public static readonly ReadOnlyDictionary<byte, string> NameMap = new( new Dictionary<byte, string>() {
				{  1, "Employee" },
				{  2, "Civil" },
				{  3, "Executive Staff" },
				{  4, "Uniformed Service" },
				{  5, "Contractor" },
				{  6, "Organizational Affiliate" },
				{  7, "Organizational Beneficiary" },
			} );
	};
}






