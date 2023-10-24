using Org.BouncyCastle.Asn1;

namespace SubjectAlternativeName.X400Address;

/*
BuiltInDomainDefinedAttributes ::=
	SEQUENCE SIZE (1..ub-domain-defined-attributes) OF BuiltInDomainDefinedAttribute
ub-domain-defined-attributes INTEGER ::= 4
*/
public class DomainDefinedAttributes
		: Asn1Encodable {
	public static DomainDefinedAttributes? GetInstance( object obj ) {
		return obj == null
			? null
			: obj is DomainDefinedAttributes domainDefinedAttributes
				? domainDefinedAttributes
				: new DomainDefinedAttributes( Asn1Sequence.GetInstance( obj ) );
	}

	public static DomainDefinedAttributes GetInstance( Asn1TaggedObject taggedObject, bool declaredExplicit ) {
		return new DomainDefinedAttributes( Asn1Sequence.GetInstance( taggedObject, declaredExplicit ) );
	}

	private readonly Asn1Sequence m_strings;

	internal DomainDefinedAttributes( Asn1Sequence seq ) {
		foreach (Asn1Encodable? element in seq) {
			if (element is not DomainDefinedAttribute)
				throw new ArgumentException( "attempt to insert non DomainDefinedAttribute into DomainDefinedAttributes" );
		}

		m_strings = seq;
	}

	public DomainDefinedAttributes( DomainDefinedAttribute p ) {
		m_strings = new DerSequence( p );
	}

	public DomainDefinedAttributes( string type, string value )
		: this( new DomainDefinedAttribute( type, value ) ) {
	}
	public DomainDefinedAttributes( (string type, string value) x )
		: this( x.type, x.value ) {
	}

	public DomainDefinedAttributes( DomainDefinedAttribute[] strs ) {
		m_strings = new DerSequence( strs );
	}

	public DomainDefinedAttributes( string[][] strs ) {
		Asn1EncodableVector v = new( strs.Length );
		for (int i = 0; i < strs.Length; i++) {
			v.Add( new DomainDefinedAttribute( strs[i] ) );
		}
		m_strings = new DerSequence( v );
	}

	public virtual int Count => m_strings.Count;

	/**
	 * Return the DomainDefinedAttribute at index.
	 *
	 * @param index index of the DomainDefinedAttribute of interest
	 * @return the DomainDefinedAttribute at index.
	 */
	public DomainDefinedAttribute this[int index] => (DomainDefinedAttribute)m_strings[index];

	/**
	 * <pre>
	 * DomainDefinedAttributes ::= SEQUENCE SIZE (1..4) OF BuiltInDomainDefinedAttribute
	 * </pre>
	 */
	public override Asn1Object ToAsn1Object() => m_strings;
}
