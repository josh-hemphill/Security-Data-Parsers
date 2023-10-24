using System.Collections.ObjectModel;
using AsnUtils;
using BinaryUtils;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using static FederalAgencySmartCredentialNumber.Segments;

/*
	For info on the spec,
	See: https://www.cac.mil/Portals/53/Documents/CAC_NG_Implementation_Guide_v2.6.pdf
	See: https://www.cac.mil/Portals/53/Documents/ref1.c.i-CAC_End-Point_Implementation_Guide_v1-22.pdf
*/

namespace FederalAgencySmartCredentialNumber;
public class FASCN {
	public FASCN( string joinedSeriesIntegers ) {
		InitFromString( joinedSeriesIntegers );
	}
	public FASCN( byte[] raw ) {
		if (raw.Length > 256) throw new Exception( "Raw bytes are also too large to be raw FASCN digits" );
		Init( raw );
	}
	public FASCN( GeneralNames SANExt ) {
		InitFromExt( SANExt );
	}
	public static FASCN FromCertificate( System.Security.Cryptography.X509Certificates.X509Certificate2 cert ) {
		return new FASCN( new X509CertificateParser()
			.ReadCertificate( cert.RawData )
			.GetSubjectAlternativeNameExtension() );
	}
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

	public const byte BCD_BYTE_LENGTH = 40;
	public const byte BCD_BIT_LENGTH = 5;
	public const int BITS_LENGTH = BCD_BIT_LENGTH * BCD_BYTE_LENGTH;

	public readonly AgencyCode agencyCode = new();
	public readonly SystemCode systemCode = new();
	public readonly CredentialNumber credentialNumber = new();
	public readonly CredentialSeries credentialSeries = new();
	public readonly IndividualCredentialIssue individualCredentialIssue = new();
	public readonly PersonIdentifier personIdentifier = new();
	public readonly OrganizationalCategory organizationalCategory = new();
	public readonly OrganizationIdentifier organizationIdentifier = new();
	public readonly PersonOrOrganizationAssociationCategory personOrOrganizationAssociationCategory = new();


	public string GetOrgIdTypeName() {
		return organizationalCategory?.Digits != null ?
		 OrgIdTypeNameMap.GetValueOrDefault( organizationalCategory.Digits[0], "Unknown" ) : "";
	}

	public static readonly ReadOnlyDictionary<byte, string> OrgIdTypeNameMap = new( new Dictionary<byte, string>() {
			{  1, "NIST SP800-87 Agency Code" },
			{  2, "State Code" },
			{  3, "Company Code" },
			{  4, "Numeric Country Code" },
		} );

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

	public class FASCNSegmentBitsMismatch : Exception {
		public FASCNSegmentBitsMismatch() {
		}

		public FASCNSegmentBitsMismatch( string message )
			 : base( message ) {
		}

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

}
