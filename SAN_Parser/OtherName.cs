using System.Formats.Asn1;
using Kerberos.NET.Entities;
using FASCN_Parser;

/*
	For info on the spec,
	See: https://datatracker.ietf.org/doc/html/rfc5280#section-4.2.1.6:~:text=GeneralName%0A%0A%20%20%20GeneralName%20%3A%3A%3D%20CHOICE%20%7B-,otherName,-%5B0%5D%20%20%20%20%20OtherName%2C%0A%20%20%20%20%20%20%20%20rfc822Name
*/

namespace SAN_Parser;

public class OtherName {
	public PrincipalName? principalName;
	public (string user, string domain)? principalUN;
	public FASCN? fASCN;
	public OtherName( AsnReader sequenceReader ) {
		AsnReader otherReader = sequenceReader.ReadSequence( AsnStructures.AsnTags.OtherName.Tag );
		while (otherReader.HasData) {
			Asn1Tag otherTag = otherReader.PeekTag();
			if (otherTag == AsnStructures.AsnTags.OtherName.ObjID) {
				string objID = otherReader.ReadObjectIdentifier( AsnStructures.AsnTags.OtherName.ObjID );

				if (objID == AsnStructures.AsnTags.OtherName.OIds.principalName.Id) {
					Asn1Tag pnTag = otherReader.PeekTag();
					AsnReader pnData = otherReader.ReadSequence( pnTag );
					PrincipalNameType? nameType = null;
					List<string> nameString = new();

					while (pnData.HasData) {
						Asn1Tag dataTag = pnData.PeekTag();
						if (dataTag == Asn1Tag.Integer) {
							bool success = pnData.TryReadInt32( out int lNameType, dataTag );
							if (!success) throw new AsnContentException( "Asn.1 Encoding of OtherName>PrincipalName>NameType integer is out of range" );
							nameType = Enum.IsDefined( typeof( PrincipalNameType ), lNameType ) ?
								(PrincipalNameType)lNameType :
								throw new AsnContentException( "Asn.1 Encoding of OtherName>PrincipalName>NameType integer is out of range" );
						} else if (dataTag == Asn1Tag.Sequence) {
							AsnReader nameData = pnData.ReadSequence( dataTag );
							while (nameData.HasData) {
								Asn1Tag nameTag = nameData.PeekTag();
								string nameSegment = nameData.ReadCharacterString( UniversalTagNumber.UTF8String, nameTag );
								nameString.Add( nameSegment );
							}
						} else if (dataTag.TagValue == (int)UniversalTagNumber.UTF8String) {
							nameString.Add( pnData.ReadCharacterString( UniversalTagNumber.UTF8String, dataTag ) );
						} else {
							_ = pnData.ReadEncodedValue();
						}
					}

					if (nameString.Count == 0) continue;
					string realm = string.Empty;
					if (nameString.Count > 2) {
						realm = nameString[2];
					}
					KrbPrincipalName krbPrincipal = nameString.Count == 1 ?
						KrbPrincipalName.FromString( nameString[0], nameType ) :
						new PrincipalName( nameType ?? TryDetectPrincipalType( nameString[0] ), realm, nameString.Take( 2 ) );

					if (realm == string.Empty && krbPrincipal.Name.Length > 2) {
						realm = krbPrincipal.Name[2];
					}

					principalName = new PrincipalName( krbPrincipal.Type, realm, krbPrincipal.Name.Take( 2 ) );

					if (principalName.FullyQualifiedName.Count( v => v == '@' ) == 1) {
						string[] nameSeg = principalName.FullyQualifiedName.Split( '@' );
						principalUN = (nameSeg[0], nameSeg[1]);
					}
					continue;
				} else if (objID == AsnStructures.AsnTags.OtherName.OIds.FASCN.Id) {
					Asn1Tag fascnTag = otherReader.PeekTag();
					AsnReader fascnData = otherReader.ReadSetOf( fascnTag );
					while (fascnData.HasData) {
						Asn1Tag dataTag = fascnData.PeekTag();
						if (dataTag == Asn1Tag.PrimitiveOctetString) {
							byte[] bits = fascnData.ReadOctetString( dataTag );
							fASCN = new FASCN( bits );
						} else if (dataTag.TagValue == (int)UniversalTagNumber.UTF8String) {
							fASCN = new FASCN( fascnData.ReadCharacterString( UniversalTagNumber.UTF8String, dataTag ) );
						} else {
							_ = fascnData.ReadEncodedValue();
						}
					}
					continue;
				}
			}
			_ = otherReader.ReadEncodedValue();
		}
	}
	private static PrincipalNameType TryDetectPrincipalType( string principal ) {
		if (principal.Contains( '/' )) {
			return PrincipalNameType.NT_SRV_HST;
		}

		if (principal.Contains( '@' )) {
			return PrincipalNameType.NT_PRINCIPAL;
		}

		if (principal.Contains( ',' )) {
			return PrincipalNameType.NT_X500_PRINCIPAL;
		}

		return PrincipalNameType.NT_ENTERPRISE;
	}
}
