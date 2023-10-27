using System.Collections.ObjectModel;
using AsnUtils;
using BinaryUtils;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using static SecurityDataParsers.FederalAgencySmartCredentialNumber.Segments;

/*
	For info on the spec,
	See: https://www.cac.mil/Portals/53/Documents/CAC_NG_Implementation_Guide_v2.6.pdf
	See: https://www.cac.mil/Portals/53/Documents/ref1.c.i-CAC_End-Point_Implementation_Guide_v1-22.pdf
*/

namespace SecurityDataParsers.FederalAgencySmartCredentialNumber;
/// <summary>
/// Represents a Federal Agency Smart Credential Number (FASC-N) and provides methods to initialize it from various sources.
/// For info on the spec, see <see href="https://www.cac.mil/Portals/53/Documents/CAC_NG_Implementation_Guide_v2.6.pdf">CAC NG Implementation Guide v2.6</see> or
/// <see href="https://www.cac.mil/Portals/53/Documents/ref1.c.i-CAC_End-Point_Implementation_Guide_v1-22.pdf">CAC End-Point Implementation Guide v1-22</see>.
/// </summary>
public class FASCN {
	/// <summary>
	/// Initializes a new instance of the <see cref="FASCN"/> class from a string of joined series integers.
	/// </summary>
	/// <param name="joinedSeriesIntegers">The string of joined series integers.</param>
	public FASCN( string joinedSeriesIntegers ) {
		InitFromString( joinedSeriesIntegers );
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="FASCN"/> class from a byte array of raw FASCN digits.
	/// </summary>
	/// <param name="raw">The byte array of raw FASCN digits.</param>
	public FASCN( byte[] raw ) {
		if (raw.Length > 256) throw new Exception( "Raw bytes are also too large to be raw FASCN digits" );
		Init( raw );
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="FASCN"/> class from a GeneralNames object.
	/// </summary>
	/// <param name="SANExt">The GeneralNames object.</param>
	public FASCN( GeneralNames SANExt ) {
		InitFromExt( SANExt );
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="FASCN"/> class from an X.509 certificate.
	/// </summary>
	/// <param name="cert">The X.509 certificate.</param>
	/// <returns>A new instance of the <see cref="FASCN"/> class.</returns>
	public static FASCN FromCertificate( System.Security.Cryptography.X509Certificates.X509Certificate2 cert ) {
		return new FASCN( new X509CertificateParser()
			.ReadCertificate( cert.RawData )
			.GetSubjectAlternativeNameExtension() );
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="FASCN"/> class from an X.509 subject alternative name extension.
	/// </summary>
	/// <param name="SANExt">The X.509 subject alternative name extension.</param>
	/// <returns>A new instance of the <see cref="FASCN"/> class.</returns>
	public static FASCN FromSAN( System.Security.Cryptography.X509Certificates.X509SubjectAlternativeNameExtension SANExt ) {
		Asn1OctetString altNames = Asn1OctetString.GetInstance( SANExt.RawData );
		Asn1Object asn1Object = X509ExtensionUtilities.FromExtensionValue( altNames );
		return new FASCN( GeneralNames.GetInstance( asn1Object ) );
	}

	private void InitFromExt( GeneralNames SANExtNames ) {
		byte[] bits = new DerOctetString( SANExtNames
			.GetNames()
			.Where( v => v.TagNo == GeneralName.OtherName )
			.Select( v => OtherName.GetInstance( v.Name ) )
			.Single( v => v.TypeID.Id == CommonOids.FASCN.Id )
			.Value )
				.GetOctets();

		Init( bits );
	}
	private void InitFromString( string joinedSeriesIntegers ) {

		byte[] canBeHex = Convert.FromHexString( joinedSeriesIntegers );
		if (joinedSeriesIntegers.Any( char.IsLetter )) {
			if (IsSegmentedHexValues( joinedSeriesIntegers ))
				SetFromDigits( joinedSeriesIntegers, true );
			else
				Init( canBeHex );
		} else {
			if (joinedSeriesIntegers.Length == 32) {
				SetFromDigits( joinedSeriesIntegers, false );
			} else
				throw new ArgumentException( "Invalid FASCN String: must be contiguous integers, contiguous hex integers, or hex encoded raw bytes" );
		}
	}

	/// <summary>
	/// The length of each BCD byte in bits.
	/// </summary>
	public const byte BCD_BYTE_LENGTH = 40;

	/// <summary>
	/// The length of each BCD digit in bits.
	/// </summary>
	public const byte BCD_BIT_LENGTH = 5;

	/// <summary>
	/// The total length of the FASCN in bits.
	/// </summary>
	public const int BITS_LENGTH = BCD_BIT_LENGTH * BCD_BYTE_LENGTH;

	/// <summary>
	/// The agency code segment of the FASCN.
	/// </summary>
	public readonly AgencyCode agencyCode = new();

	/// <summary>
	/// The system code segment of the FASCN.
	/// </summary>
	public readonly SystemCode systemCode = new();

	/// <summary>
	/// The credential number segment of the FASCN.
	/// </summary>
	public readonly CredentialNumber credentialNumber = new();

	/// <summary>
	/// The credential series segment of the FASCN.
	/// </summary>
	public readonly CredentialSeries credentialSeries = new();

	/// <summary>
	/// The individual credential issue segment of the FASCN.
	/// </summary>
	public readonly IndividualCredentialIssue individualCredentialIssue = new();

	/// <summary>
	/// The person identifier segment of the FASCN.
	/// </summary>
	public readonly PersonIdentifier personIdentifier = new();

	/// <summary>
	/// The organizational category segment of the FASCN.
	/// </summary>
	public readonly OrganizationalCategory organizationalCategory = new();

	/// <summary>
	/// The organization identifier segment of the FASCN.
	/// </summary>
	public readonly OrganizationIdentifier organizationIdentifier = new();

	/// <summary>
	/// The person or organization association category segment of the FASCN.
	/// </summary>
	public readonly PersonOrOrganizationAssociationCategory personOrOrganizationAssociationCategory = new();


	/// <summary>
	/// Returns the name of the organization identifier type based on the organizational category segment of the FASCN.
	/// </summary>
	/// <returns>The name of the organization identifier type.</returns>
	public string GetOrgIdTypeName() {
		return organizationalCategory?.Digits != null ?
		 OrgIdTypeNameMap.GetValueOrDefault( organizationalCategory.Digits[0], "Unknown" ) : "";
	}

	/// <summary>
	/// A dictionary that maps the first digit of the organizational category segment of the FASCN to the name of the organization identifier type.
	/// </summary>
	public static readonly ReadOnlyDictionary<byte, string> OrgIdTypeNameMap = new( new Dictionary<byte, string>() {
			{  1, "NIST SP800-87 Agency Code" },
			{  2, "State Code" },
			{  3, "Company Code" },
			{  4, "Numeric Country Code" },
		} );

	/// <summary>
	/// The series of properties that make up the FASCN.
	/// </summary>
	public ISegment[] PropertySeries => new ISegment[] {
		StartSentinel,
		agencyCode,
		FieldSeparator,
		systemCode,
		FieldSeparator,
		credentialNumber,
		FieldSeparator,
		credentialSeries,
		FieldSeparator,
		individualCredentialIssue,
		FieldSeparator,
		personIdentifier,
		organizationalCategory,
		organizationIdentifier,
		personOrOrganizationAssociationCategory,
		EndSentinel
	};

	private bool IsSegmentedHexValues( string b ) {
		int i = -1;
		return PropertySeries.All( v => {
			i++;
			return v is FlagSegment f ? b[i] == f.MustEqual.ToString( "X" )[0] : char.IsDigit( b[i] );
		} );
	}
	private void SetFromDigits( string digits, bool hasSeparators ) {
		int i = 0;
		foreach (ISegment v in PropertySeries.Where( k => !(k is FlagSegment && hasSeparators) )) {
			if (v is IValueSegment f) {
				for (int d = 0; d < v.Length; d++) {
					f.Digits[d] = byte.Parse( digits[i].ToString(), System.Globalization.NumberStyles.HexNumber );
					i++;
				}
			}
			i++;
		}
	}

	/// <summary>
	/// Exception thrown when there is a bit length mismatch in a FASCN segment.
	/// </summary>
	public class FASCNSegmentBitsMismatch : Exception {
		/// <summary>
		/// Initializes a new instance of the <see cref="FASCNSegmentBitsMismatch"/> class.
		/// </summary>
		public FASCNSegmentBitsMismatch() {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FASCNSegmentBitsMismatch"/> class with a specified error message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public FASCNSegmentBitsMismatch( string message )
			: base( message ) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FASCNSegmentBitsMismatch"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="inner">The exception that is the cause of the current exception.</param>
		public FASCNSegmentBitsMismatch( string message, Exception inner )
			: base( message, inner ) {
		}
	}

	private static (bool valid, byte value) GetBCD( bool[] bits, int bitI ) {
		bool[] lBCD = new bool[]{
			bits[0 + bitI],
			bits[1 + bitI],
			bits[2 + bitI],
			bits[3 + bitI]
		};
		bool parity = bits[4 + bitI];

		bool lParity = lBCD.Where( v => v ).ToArray().Length % 2 != 1;

		return lParity != parity ?
			((bool valid, byte value))(false, 0) :
			((bool valid, byte value))(true, Binary.BoolArrayReverseToByte( lBCD ));
	}

	private void Init( byte[] bits ) {
		bool[] lBits = Binary.GetBytesAsBoolArr( bits );

		bool found = false;
		int bitI = 0;
		for (; bitI < lBits.Length - 5; bitI++) {
			(bool valid, byte value) = GetBCD( lBits, bitI );

			if (!valid || value != StartSentinel.MustEqual) continue;
			found = true;
			break;
		}
		if (!found) throw new Exception();

		for (int i = bitI, seriesIndex = 0; i < BITS_LENGTH && seriesIndex < PropertySeries.Length; seriesIndex++) {
			ISegment segment = PropertySeries[seriesIndex];

			if (segment is FlagSegment fSegment) {
				(bool valid, byte value) = GetBCD( lBits, i );
				if (!valid || value != fSegment.MustEqual) {
					throw new InvalidDataException( "FASC-N flag position has wrong bits" );
				}
			} else if (segment is IValueSegment vSegment) {
				byte[] segVal = new byte[vSegment.Length];
				for (byte segByte = 0; segByte < vSegment.Length; segByte++) {
					int offset = Binary.MathWithinByte( i + (segByte * BCD_BIT_LENGTH) );
					(bool valid, byte value) = GetBCD( lBits, offset );

					if (!valid || value > 9) {
						throw new FASCNSegmentBitsMismatch( "Unknown Bit Segment" );
					}
					segVal[segByte] = value;
				}
				vSegment.Digits = segVal;
			}
			i = Binary.MathWithinByte( i + (segment.Length * BCD_BIT_LENGTH) );
		}
	}



	/// <summary>
	/// Deconstructs the FASCN into its individual segments.
	/// </summary>
	/// <param name="agencyCode">The agency code segment.</param>
	/// <param name="systemCode">The system code segment.</param>
	public void Deconstruct(
		out AgencyCode agencyCode,
		out SystemCode systemCode
	) {
		agencyCode = this.agencyCode;
		systemCode = this.systemCode;
	}

	/// <summary>
	/// Deconstructs the FASCN into its individual segments.
	/// </summary>
	/// <param name="agencyCode">The agency code segment.</param>
	/// <param name="systemCode">The system code segment.</param>
	/// <param name="credentialNumber">The credential number segment.</param>
	public void Deconstruct(
		out AgencyCode agencyCode,
		out SystemCode systemCode,
		out CredentialNumber credentialNumber
	) {
		agencyCode = this.agencyCode;
		systemCode = this.systemCode;
		credentialNumber = this.credentialNumber;
	}

	/// <summary>
	/// Deconstructs the FASCN into its individual segments.
	/// </summary>
	/// <param name="agencyCode">The agency code segment.</param>
	/// <param name="systemCode">The system code segment.</param>
	/// <param name="credentialNumber">The credential number segment.</param>
	/// <param name="credentialSeries">The credential series segment.</param>
	public void Deconstruct(
		out AgencyCode agencyCode,
		out SystemCode systemCode,
		out CredentialNumber credentialNumber,
		out CredentialSeries credentialSeries
	) {
		agencyCode = this.agencyCode;
		systemCode = this.systemCode;
		credentialNumber = this.credentialNumber;
		credentialSeries = this.credentialSeries;
	}
	/// <summary>
	/// Deconstructs the FASCN into its individual segments.
	/// </summary>
	/// <param name="agencyCode">The agency code segment.</param>
	/// <param name="systemCode">The system code segment.</param>
	/// <param name="credentialNumber">The credential number segment.</param>
	/// <param name="credentialSeries">The credential series segment.</param>
	/// <param name="individualCredentialIssue">The individual credential issue segment.</param>
	public void Deconstruct(
		out AgencyCode agencyCode,
		out SystemCode systemCode,
		out CredentialNumber credentialNumber,
		out CredentialSeries credentialSeries,
		out IndividualCredentialIssue individualCredentialIssue
	) {
		agencyCode = this.agencyCode;
		systemCode = this.systemCode;
		credentialNumber = this.credentialNumber;
		credentialSeries = this.credentialSeries;
		individualCredentialIssue = this.individualCredentialIssue;
	}
	/// <summary>
	/// Deconstructs the FASCN into its individual segments.
	/// </summary>
	/// <param name="agencyCode">The agency code segment.</param>
	/// <param name="systemCode">The system code segment.</param>
	/// <param name="credentialNumber">The credential number segment.</param>
	/// <param name="credentialSeries">The credential series segment.</param>
	/// <param name="individualCredentialIssue">The individual credential issue segment.</param>
	/// <param name="personIdentifier">The person identifier segment.</param>
	public void Deconstruct(
		out AgencyCode agencyCode,
		out SystemCode systemCode,
		out CredentialNumber credentialNumber,
		out CredentialSeries credentialSeries,
		out IndividualCredentialIssue individualCredentialIssue,
		out PersonIdentifier personIdentifier

	) {
		agencyCode = this.agencyCode;
		systemCode = this.systemCode;
		credentialNumber = this.credentialNumber;
		credentialSeries = this.credentialSeries;
		individualCredentialIssue = this.individualCredentialIssue;
		personIdentifier = this.personIdentifier;
	}

	/// <summary>
	/// Deconstructs the FASCN into its individual segments.
	/// </summary>
	/// <param name="agencyCode">The agency code segment.</param>
	/// <param name="systemCode">The system code segment.</param>
	/// <param name="credentialNumber">The credential number segment.</param>
	/// <param name="credentialSeries">The credential series segment.</param>
	/// <param name="individualCredentialIssue">The individual credential issue segment.</param>
	/// <param name="personIdentifier">The person identifier segment.</param>
	/// <param name="organizationalCategory">The organizational category segment.</param>
	public void Deconstruct(
		out AgencyCode agencyCode,
		out SystemCode systemCode,
		out CredentialNumber credentialNumber,
		out CredentialSeries credentialSeries,
		out IndividualCredentialIssue individualCredentialIssue,
		out PersonIdentifier personIdentifier,
		out OrganizationalCategory organizationalCategory
	) {
		agencyCode = this.agencyCode;
		systemCode = this.systemCode;
		credentialNumber = this.credentialNumber;
		credentialSeries = this.credentialSeries;
		individualCredentialIssue = this.individualCredentialIssue;
		personIdentifier = this.personIdentifier;
		organizationalCategory = this.organizationalCategory;
	}

	/// <summary>
	/// Deconstructs the FASCN into its individual segments.
	/// </summary>
	/// <param name="agencyCode">The agency code segment.</param>
	/// <param name="systemCode">The system code segment.</param>
	/// <param name="credentialNumber">The credential number segment.</param>
	/// <param name="credentialSeries">The credential series segment.</param>
	/// <param name="individualCredentialIssue">The individual credential issue segment.</param>
	/// <param name="personIdentifier">The person identifier segment.</param>
	/// <param name="organizationalCategory">The organizational category segment.</param>
	/// <param name="organizationIdentifier">The organization identifier segment.</param>
	public void Deconstruct(
		out AgencyCode agencyCode,
		out SystemCode systemCode,
		out CredentialNumber credentialNumber,
		out CredentialSeries credentialSeries,
		out IndividualCredentialIssue individualCredentialIssue,
		out PersonIdentifier personIdentifier,
		out OrganizationalCategory organizationalCategory,
		out OrganizationIdentifier organizationIdentifier
	) {
		agencyCode = this.agencyCode;
		systemCode = this.systemCode;
		credentialNumber = this.credentialNumber;
		credentialSeries = this.credentialSeries;
		individualCredentialIssue = this.individualCredentialIssue;
		personIdentifier = this.personIdentifier;
		organizationalCategory = this.organizationalCategory;
		organizationIdentifier = this.organizationIdentifier;
	}

	/// <summary>
	/// Deconstructs the FASCN into its individual segments.
	/// </summary>
	/// <param name="agencyCode">The agency code segment.</param>
	/// <param name="systemCode">The system code segment.</param>
	/// <param name="credentialNumber">The credential number segment.</param>
	/// <param name="credentialSeries">The credential series segment.</param>
	/// <param name="individualCredentialIssue">The individual credential issue segment.</param>
	/// <param name="personIdentifier">The person identifier segment.</param>
	/// <param name="organizationalCategory">The organizational category segment.</param>
	/// <param name="organizationIdentifier">The organization identifier segment.</param>
	/// <param name="personOrOrganizationAssociationCategory">The person or organization association category segment.</param>
	public void Deconstruct(
		out AgencyCode agencyCode,
		out SystemCode systemCode,
		out CredentialNumber credentialNumber,
		out CredentialSeries credentialSeries,
		out IndividualCredentialIssue individualCredentialIssue,
		out PersonIdentifier personIdentifier,
		out OrganizationalCategory organizationalCategory,
		out OrganizationIdentifier organizationIdentifier,
		out PersonOrOrganizationAssociationCategory personOrOrganizationAssociationCategory
	) {
		agencyCode = this.agencyCode;
		systemCode = this.systemCode;
		credentialNumber = this.credentialNumber;
		credentialSeries = this.credentialSeries;
		individualCredentialIssue = this.individualCredentialIssue;
		personIdentifier = this.personIdentifier;
		organizationalCategory = this.organizationalCategory;
		organizationIdentifier = this.organizationIdentifier;
		personOrOrganizationAssociationCategory = this.personOrOrganizationAssociationCategory;
	}
}
