using System.Formats.Asn1;
using Org.BouncyCastle.Asn1;

namespace SubjectAlternativeName.X400Address;

/*
ORAddress ::= SEQUENCE {
	built-in-standard-attributes			BuiltInStandardAttributes,
	built-in-domain-defined-attributes	BuiltInDomainDefinedAttributes	OPTIONAL,
	-- see also teletex-domain-defined-attributes
	extension-attributes						ExtensionAttributes					OPTIONAL
}
*/
public class ORAddress
		: Asn1Encodable {
	private readonly Asn1Sequence seq;
	public readonly StandardAttributes? standardAttributes;
	public readonly DomainDefinedAttributes? domainDefinedAttributes;
	public readonly ExtensionAttributes? extensionAttributes;
	public static ORAddress? GetInstance( object obj ) {
		return obj == null
			? null
			: obj is ORAddress oRAddress
				? oRAddress
				: new ORAddress( Asn1Sequence.GetInstance( obj ) );
	}

	public static ORAddress GetInstance( Asn1TaggedObject taggedObject, bool declaredExplicit ) {
		return new ORAddress( Asn1Sequence.GetInstance( taggedObject, declaredExplicit ) );
	}

	private static bool IsSameTag( Asn1TaggedObject x, Asn1Tag z ) => x.TagNo == z.TagValue && x.TagClass == (int)z.TagClass;
	public ORAddress( Asn1Sequence seq ) {
		this.seq = seq;
		standardAttributes = StandardAttributes.GetInstance( seq[0] );
		if (seq[1] != null) domainDefinedAttributes = DomainDefinedAttributes.GetInstance( seq[1] );
		if (seq[2] != null) extensionAttributes = ExtensionAttributes.GetInstance( seq[2] );
	}

	public ORAddress( Asn1Encodable[] strs )
		: this( new DerSequence( strs ) ) {
	}

	/**
	 * <pre>
	 * ORAddress ::= SEQUENCE SIZE (1..MAX) OF PrintableString
	 * </pre>
	 */
	public override Asn1Object ToAsn1Object() => seq;
}
