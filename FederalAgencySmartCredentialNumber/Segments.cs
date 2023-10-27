using System.Collections.ObjectModel;

namespace SecurityDataParsers.FederalAgencySmartCredentialNumber;

/// <summary>
/// Contains classes and interfaces representing the segments of a Federal Agency Smart Credential Number.
/// </summary>
/// <remarks>
/// A Federal Agency Smart Credential Number is a unique identifier assigned to individuals who require access to secure federal facilities and information systems.
/// However the spec allows for the use of the credential number to be used for other purposes as well.
/// Including but not limited to:
/// </remarks>
public static class Segments {
	/// <summary>
	/// Represents the length of the flag segment.
	/// </summary>
	/// <remarks>
	/// The flag segment is always a single character.
	/// </remarks>
	public static readonly byte FlagLength = 1;

	/// <summary>
	/// Represents a segment of a Federal Agency Smart Credential Number.
	/// </summary>
	public interface ISegment {
		/// <summary>
		/// Gets the length of the segment.
		/// </summary>
		public byte Length { get; }
	}

	/// <summary>
	/// Represents a segment of a Federal Agency Smart Credential Number that contains a flag value.
	/// </summary>
	public class FlagSegment : ISegment {
		/// <summary>
		/// Gets the length of the flag segment.
		/// </summary>
		public byte Length { get; } = FlagLength;

		/// <summary>
		/// Gets the value that the flag segment must equal.
		/// </summary>
		public int MustEqual { get; init; }
	}

	/// <summary>
	/// Represents a segment of a Federal Agency Smart Credential Number that contains a value.
	/// </summary>
	public interface IValueSegment : ISegment {
		/// <summary>
		/// Gets or sets the digits that make up the value of the segment.
		/// </summary>
		/// <remarks>
		/// The number of digits in the segment is determined by the value of the Length property.
		/// </remarks>
		public byte[] Digits { get; set; }

		/// <summary>
		/// Gets the name of the field represented by the segment.
		/// </summary>
		public abstract string FieldName { get; }

		/// <summary>
		/// Gets the abbreviation for the field represented by the segment.
		/// </summary>
		public abstract string Abbreviation { get; }
	}

	/// <summary>
	/// Represents a segment of a Federal Agency Smart Credential Number that contains an enumerated value.
	/// </summary>
	public interface IEnumSegment {
		/// <summary>
		/// Gets a friendly name for the enumerated value represented by the segment.
		/// </summary>
		public string GetFriendlyName();
	}

	/// <summary>
	/// Represents a segment of a Federal Agency Smart Credential Number that will have its digits destructured.
	/// </summary>
	public interface ITupleSegment<T> {
		/// <summary>
		/// Gets the value of the segment digits as a TupleValue.
		/// </summary>
		public T AsTuple();
	}

	private static readonly Func<int, FlagSegment> IntToFlag = ( b ) => new() { MustEqual = b };

	/// <summary>
	/// Represents the start sentinel of a Federal Agency Smart Credential Number.
	/// </summary>
	public static readonly FlagSegment StartSentinel = IntToFlag( 0xB );

	/// <summary>
	/// Represents the field separator of a Federal Agency Smart Credential Number.
	/// </summary>
	public static readonly FlagSegment FieldSeparator = IntToFlag( 0xD );

	/// <summary>
	/// Represents the end sentinel of a Federal Agency Smart Credential Number.
	/// </summary>
	public static readonly FlagSegment EndSentinel = IntToFlag( 0xF );

	/// <summary>
	/// Represents a segment of a Federal Agency Smart Credential Number that contains an agency code value.
	/// </summary>
	public class AgencyCode : IValueSegment, IEnumSegment {
		/// <summary>
		/// Gets the length of the agency code segment.
		/// </summary>
		public byte Length { get; } = 4;

		/// <summary>
		/// Gets or sets the digits that make up the value of the agency code segment.
		/// </summary>
		public byte[] Digits { get; set; } = Array.Empty<byte>();

		/// <summary>
		/// Gets the name of the agency code field.
		/// </summary>
		public string FieldName { get; } = "Agency Code";

		/// <summary>
		/// Gets the abbreviation for the agency code field.
		/// </summary>
		public string Abbreviation { get; } = "AC";

		/// <summary>
		/// Gets a friendly name for the agency code value represented by the segment.
		/// </summary>
		public string GetFriendlyName() {
			return this?.Digits != null ? NameMap.GetValueOrDefault( (Digits[0], Digits[1], Digits[2], Digits[3]), "Unknown" ) : "";
		}

		/// <summary>
		/// A dictionary that maps agency code values to friendly names.
		/// </summary>
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

	/// <summary>
	/// Represents a segment of a Federal Agency Smart Credential Number that contains a system code value.
	/// </summary>
	public class SystemCode : IValueSegment, ITupleSegment<(byte, byte, byte, byte)> {
		/// <summary>
		/// Gets the length of the system code segment.
		/// </summary>
		public byte Length { get; } = 4;

		/// <summary>
		/// Gets or sets the digits that make up the value of the system code segment.
		/// </summary>
		public byte[] Digits { get; set; } = Array.Empty<byte>();


		/// <inheritdoc/>
		public (byte, byte, byte, byte) AsTuple() {
			return (Digits[0], Digits[1], Digits[2], Digits[3]);
		}

		/// <summary>
		/// Gets the name of the system code field.
		/// </summary>
		public string FieldName { get; } = "System Code";

		/// <summary>
		/// Gets the abbreviation for the system code field.
		/// </summary>
		public string Abbreviation { get; } = "SC";
	};

	/// <summary>
	/// Represents a segment of a Federal Agency Smart Credential Number that contains a credential number value.
	/// </summary>
	public class CredentialNumber : IValueSegment, ITupleSegment<(byte, byte, byte, byte, byte, byte)> {
		/// <summary>
		/// Gets the length of the credential number segment.
		/// </summary>
		public byte Length { get; } = 6;

		/// <summary>
		/// Gets or sets the digits that make up the value of the credential number segment.
		/// </summary>
		public byte[] Digits { get; set; } = Array.Empty<byte>();

		/// <inheritdoc/>
		public (byte, byte, byte, byte, byte, byte) AsTuple() {
			return (Digits[0], Digits[1], Digits[2], Digits[3], Digits[4], Digits[5]);
		}

		/// <summary>
		/// Gets the name of the credential number field.
		/// </summary>
		public string FieldName { get; } = "Credential Number";

		/// <summary>
		/// Gets the abbreviation for the credential number field.
		/// </summary>
		public string Abbreviation { get; } = "CN";
	};

	/// <summary>
	/// Represents a segment of a Federal Agency Smart Credential Number that contains a credential series value.
	/// </summary>
	public class CredentialSeries : IValueSegment, ITupleSegment<ValueTuple<byte>> {
		/// <summary>
		/// Gets the length of the credential series segment.
		/// </summary>
		public byte Length { get; } = 1;

		/// <summary>
		/// Gets or sets the digits that make up the value of the credential series segment.
		/// </summary>
		public byte[] Digits { get; set; } = Array.Empty<byte>();

		/// <inheritdoc/>
		public ValueTuple<byte> AsTuple() {
			return ValueTuple.Create( Digits[0] );
		}

		/// <summary>
		/// Gets the name of the credential series field.
		/// </summary>
		public string FieldName { get; } = "Credential Series";

		/// <summary>
		/// Gets the abbreviation for the credential series field.
		/// </summary>
		public string Abbreviation { get; } = "CS";
	};

	/// <summary>
	/// Represents a segment of a Federal Agency Smart Credential Number that contains an individual credential issue value.
	/// </summary>
	public class IndividualCredentialIssue : IValueSegment, ITupleSegment<ValueTuple<byte>> {
		/// <summary>
		/// Gets the length of the individual credential issue segment.
		/// </summary>
		public byte Length { get; } = 1;

		/// <summary>
		/// Gets or sets the digits that make up the value of the individual credential issue segment.
		/// </summary>
		public byte[] Digits { get; set; } = Array.Empty<byte>();

		/// <inheritdoc/>
		public ValueTuple<byte> AsTuple() {
			return ValueTuple.Create( Digits[0] );
		}

		/// <summary>
		/// Gets the name of the individual credential issue field.
		/// </summary>
		public string FieldName { get; } = "Individual Credential Issue";

		/// <summary>
		/// Gets the abbreviation for the individual credential issue field.
		/// </summary>
		public string Abbreviation { get; } = "ICI";
	};

	/// <summary>
	/// Represents a segment of a Federal Agency Smart Credential Number that contains a person identifier value.
	/// </summary>
	public class PersonIdentifier : IValueSegment, ITupleSegment<(byte, byte, byte, byte, byte, byte, byte, byte, byte, byte)> {
		/// <summary>
		/// Gets the length of the person identifier segment.
		/// </summary>
		public byte Length { get; } = 10;

		/// <summary>
		/// Gets or sets the digits that make up the value of the person identifier segment.
		/// </summary>
		public byte[] Digits { get; set; } = Array.Empty<byte>();

		/// <inheritdoc/>
		public (byte, byte, byte, byte, byte, byte, byte, byte, byte, byte) AsTuple() {
			return (Digits[0], Digits[1], Digits[2], Digits[3], Digits[4], Digits[5], Digits[6], Digits[7], Digits[8], Digits[9]);
		}

		/// <summary>
		/// Gets the name of the person identifier field.
		/// </summary>
		public string FieldName { get; } = "Person Identifier";

		/// <summary>
		/// Gets the abbreviation for the person identifier field.
		/// </summary>
		public string Abbreviation { get; } = "PI";
	};
	/// <summary>
	/// Represents a segment of a Federal Agency Smart Credential Number that contains an organizational category value.
	/// </summary>
	public class OrganizationalCategory : IValueSegment, IEnumSegment {
		/// <summary>
		/// Gets the length of the organizational category segment.
		/// </summary>
		public byte Length { get; } = 1;

		/// <summary>
		/// Gets or sets the digits that make up the value of the organizational category segment.
		/// </summary>
		public byte[] Digits { get; set; } = Array.Empty<byte>();

		/// <summary>
		/// Gets the name of the organizational category field.
		/// </summary>
		public string FieldName { get; } = "Organizational Category";

		/// <summary>
		/// Gets the abbreviation for the organizational category field.
		/// </summary>
		public string Abbreviation { get; } = "OC";

		/// <summary>
		/// Gets a friendly name for the organizational category value.
		/// </summary>
		/// <returns>A friendly name for the organizational category value.</returns>
		public string GetFriendlyName() {
			return this?.Digits != null ? NameMap.GetValueOrDefault( Digits[0], "Unknown" ) : "";
		}

		/// <summary>
		/// Gets a dictionary that maps organizational category values to friendly names.
		/// </summary>
		public static readonly ReadOnlyDictionary<byte, string> NameMap = new( new Dictionary<byte, string>() {
				{  1, "Federal Government Agency" },
				{  2, "State Government Agency" },
				{  3, "Commercial Enterprise" },
				{  4, "Foreign Government" },
			} );
	};

	/// <summary>
	/// Represents a segment of a Federal Agency Smart Credential Number that contains an organization identifier value.
	/// </summary>
	public class OrganizationIdentifier : IValueSegment, ITupleSegment<(byte, byte, byte, byte)> {
		/// <summary>
		/// Gets the length of the organization identifier segment.
		/// </summary>
		public byte Length { get; } = 4;

		/// <summary>
		/// Gets or sets the digits that make up the value of the organization identifier segment.
		/// </summary>
		public byte[] Digits { get; set; } = Array.Empty<byte>();

		/// <inheritdoc/>
		public (byte, byte, byte, byte) AsTuple() {
			return (Digits[0], Digits[1], Digits[2], Digits[3]);
		}


		/// <summary>
		/// Gets the name of the organization identifier field.
		/// </summary>
		public string FieldName { get; } = "Organization Identifier";

		/// <summary>
		/// Gets the abbreviation for the organization identifier field.
		/// </summary>
		public string Abbreviation { get; } = "OI";
	};
	/// <summary>
	/// Represents a segment of a Federal Agency Smart Credential Number that contains a person or organization association category value.
	/// </summary>
	public class PersonOrOrganizationAssociationCategory : IValueSegment, IEnumSegment {
		/// <summary>
		/// Gets the length of the person or organization association category segment.
		/// </summary>
		public byte Length { get; } = 1;

		/// <summary>
		/// Gets or sets the digits that make up the value of the person or organization association category segment.
		/// </summary>
		public byte[] Digits { get; set; } = Array.Empty<byte>();

		/// <summary>
		/// Gets the name of the person or organization association category field.
		/// </summary>
		public string FieldName { get; } = "Person/Organization Association Category";

		/// <summary>
		/// Gets the abbreviation for the person or organization association category field.
		/// </summary>
		public string Abbreviation { get; } = "POA";

		/// <summary>
		/// Gets a friendly name for the person or organization association category value.
		/// </summary>
		/// <returns>A friendly name for the person or organization association category value.</returns>
		public string GetFriendlyName() {
			return this?.Digits != null ? NameMap.GetValueOrDefault( Digits[0], "Unknown" ) : "";
		}

		/// <summary>
		/// Gets a dictionary that maps person or organization association category values to friendly names.
		/// </summary>
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






