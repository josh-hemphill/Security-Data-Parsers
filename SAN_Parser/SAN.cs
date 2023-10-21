using System.Formats.Asn1;
using System.Security.Cryptography.X509Certificates;
using Kerberos.NET.Entities;
using FASCN_Parser;

/*
	For info on the spec,
	See: https://datatracker.ietf.org/doc/html/rfc5280#section-4.2.1.6
*/


namespace SAN_Parser;
public class SAN {

	public SAN( X509Certificate2 cert ) {
		ParseKnownFields( GetSANExtension( cert ) );
	}
	public SAN( X509SubjectAlternativeNameExtension SANExt ) {
		ParseKnownFields( SANExt );
	}

	public static X509SubjectAlternativeNameExtension GetSANExtension( X509Certificate2 cert ) {
		foreach (X509Extension ext in cert.Extensions) {
			if (ext.Oid is not null && ext.Oid.FriendlyName == "Subject Alternative Name") {
				return new X509SubjectAlternativeNameExtension( ext.RawData );
			}
		}
		throw new MissingFieldException( "Certificate has no X509Extension for 'Subject Alternative Name'" );
	}

	public List<string> URNs = new();
	public List<OtherName> otherNames = new();
	public PrincipalName? principalName;
	public (string user, string domain)? principalUN;
	public FASCN? fASCN;
	public string? urnUuid;

	public void ParseKnownFields( X509SubjectAlternativeNameExtension SANExt ) {
		AsnReader asnReader = new( SANExt.RawData, AsnEncodingRules.DER );
		AsnReader sequenceReader = asnReader.ReadSequence( Asn1Tag.Sequence );

		while (sequenceReader.HasData) {
			Asn1Tag tag = sequenceReader.PeekTag();
			if (tag.Equals( AsnStructures.AsnTags.Uri )) {
				string urn = sequenceReader.ReadCharacterString( UniversalTagNumber.IA5String, AsnStructures.AsnTags.Uri );
				URNs.Add( urn );
				Uri uri = new( urn );
				Uri uuid = new( uri.AbsolutePath );
				if (uuid.Scheme == "uuid" && uuid.AbsolutePath.Length > 0) urnUuid = uuid.AbsolutePath;

			} else if (tag.Equals( AsnStructures.AsnTags.OtherName.Tag )) {
				otherNames.Add( new OtherName( sequenceReader ) );
			} else {
				_ = sequenceReader.ReadEncodedValue();
			}
		}
		foreach (OtherName n in otherNames) {
			principalName ??= n.principalName;
			principalUN ??= n.principalUN;
			fASCN ??= n.fASCN;
		}
	}
}
