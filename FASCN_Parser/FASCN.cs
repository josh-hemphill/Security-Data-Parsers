using System.Collections.ObjectModel;
using System.Formats.Asn1;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Utils;
using static FASCN_Parser.Segments;

/*
	For info on the spec,
	See: https://www.cac.mil/Portals/53/Documents/CAC_NG_Implementation_Guide_v2.6.pdf
	See: https://www.cac.mil/Portals/53/Documents/ref1.c.i-CAC_End-Point_Implementation_Guide_v1-22.pdf
*/

namespace FASCN_Parser;
public class FASCN {
	public FASCN( byte[] bytes ) {
		Init( bytes );
	}
	public FASCN( string joinedSeriesIntegers ) {
		InitFromString( joinedSeriesIntegers );
	}

	public FASCN( X509SubjectAlternativeNameExtension SANExt ) {
		InitFromExt( SANExt );
	}

	public FASCN( X509Certificate2 cert ) {
		foreach (X509Extension ext in cert.Extensions) {
			if (ext.Oid is not null && OidToTuple( ext.Oid ).Equals( T_SAN )) {
				InitFromExt( new X509SubjectAlternativeNameExtension( ext.RawData ) );
			}
		}
	}
	public static (string?, string?) OidToTuple( Oid x ) {
		return (x.Value, x.FriendlyName);
	}
	public static readonly Oid SAN = new( "2.5.29.35", "Subject Alternative Name" );
	public static readonly (string?, string?) T_SAN = OidToTuple( SAN );
	private static readonly Asn1Tag ConstructedContext = new( TagClass.ContextSpecific, tagValue: 0, isConstructed: true );
	private void InitFromExt( X509SubjectAlternativeNameExtension SANExt ) {
		AsnReader asnReader = new( SANExt.RawData, AsnEncodingRules.DER );
		AsnReader sequenceReader = asnReader.ReadSequence( Asn1Tag.Sequence );

		while (sequenceReader.HasData) {
			Asn1Tag tag = sequenceReader.PeekTag();
			if (tag.Equals( ConstructedContext )) {
				AsnReader otherReader = sequenceReader.ReadSequence( ConstructedContext );
				Asn1Tag otherTag = otherReader.PeekTag();
				if (otherTag == Asn1Tag.ObjectIdentifier) {
					string objID = otherReader.ReadObjectIdentifier( Asn1Tag.ObjectIdentifier );
					if (objID == "2.16.840.1.101.3.6.6") {
						Asn1Tag fascnTag = otherReader.PeekTag();
						AsnReader fascnData = otherReader.ReadSetOf( fascnTag );
						Asn1Tag dataTag = fascnData.PeekTag();
						if (dataTag == Asn1Tag.PrimitiveOctetString) {
							byte[] bits = fascnData.ReadOctetString( dataTag );
							Init( bits );
							return;
						} else if (dataTag.TagValue == (int)UniversalTagNumber.UTF8String) {
							InitFromString( fascnData.ReadCharacterString( UniversalTagNumber.UTF8String, dataTag ) );
							return;
						} else {
							_ = fascnData.ReadEncodedValue();
						}

					}
				} else {
					_ = sequenceReader.ReadEncodedValue();
				}
			}
		}
		throw new Exception( "Could not find any FASCN in SAN" );
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
	public const int BITS_LENGTH = BCD.BIT_LENGTH * BCD_BYTE_LENGTH;

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
			return v is FlagSegment f ? (b[i] == f.MustEqual.ToString( "X" )[0]) : char.IsDigit( b[i] );
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

		bool lParity = (lBCD.Where( v => v ).ToArray().Length % 2) != 1;

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

			if (!valid || (value != StartSentinel.MustEqual)) continue;
			found = true;
			break;
		}
		if (!found) throw new Exception();

		for (int i = bitI, seriesIndex = 0; (i < BITS_LENGTH) && (seriesIndex < PropertySeries.Length); seriesIndex++) {
			ISegment segment = PropertySeries[seriesIndex];

			if (segment is FlagSegment fSegment) {
				(bool valid, byte value) = GetBCD( lBits, i );
				if (!valid || (value != fSegment.MustEqual)) {
					throw new InvalidDataException( "FASC-N flag position has wrong bits" );
				}
			} else if (segment is IValueSegment vSegment) {
				byte[] segVal = new byte[vSegment.Length];
				for (byte segByte = 0; segByte < vSegment.Length; segByte++) {
					int offset = Binary.MathWithinByte( i + (segByte * BCD.BIT_LENGTH) );
					(bool valid, byte value) = GetBCD( lBits, offset );

					if (!valid || value > 9) {
						throw new FASCNSegmentBitsMismatch( "Unknown Bit Segment" );
					}
					segVal[segByte] = value;
				}
				vSegment.Digits = segVal;
			}
			i = Binary.MathWithinByte( i + (segment.Length * BCD.BIT_LENGTH) );
		}
	}

}
