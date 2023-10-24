using Org.BouncyCastle.Asn1;

namespace SubjectAlternativeName.X400Address;

/*
BuiltInDomainDefinedAttribute ::= SEQUENCE {
	type PrintableString (SIZE (1..ub-domain-defined-attribute-type-length)),
	value PrintableString (SIZE (1..ub-domain-defined-attribute-value-length))
}
ub-domain-defined-attribute-type-length INTEGER ::= 8
ub-domain-defined-attribute-value-length INTEGER ::= 128
*/
public class DomainDefinedAttribute
		: Asn1Encodable {
	public static DomainDefinedAttribute? GetInstance( object obj ) {
		return obj == null
			? null
			: obj is DomainDefinedAttribute domainDefinedAttribute
				? domainDefinedAttribute
				: new DomainDefinedAttribute( Asn1Sequence.GetInstance( obj ) );
	}

	public static DomainDefinedAttribute GetInstance( Asn1TaggedObject taggedObject, bool declaredExplicit ) {
		return new DomainDefinedAttribute( Asn1Sequence.GetInstance( taggedObject, declaredExplicit ) );
	}

	private readonly Asn1Sequence m_strings;

	public virtual DerPrintableString Type => DerPrintableString.GetInstance( m_strings[0] );
	public virtual DerPrintableString Value => DerPrintableString.GetInstance( m_strings[1] );


	internal DomainDefinedAttribute( Asn1Sequence seq ) {
		if (seq.Count != 2) throw new ArgumentException( "incorrect number of arguments, DomainDefinedAttribute requires 2 strings (type, value)" );
		foreach (Asn1Encodable? element in seq) {
			if (element is not DerPrintableString)
				throw new ArgumentException( "attempt to insert non PrintableString into DomainDefinedAttribute" );
		}

		m_strings = seq;
	}

	public DomainDefinedAttribute( DerPrintableString type, DerPrintableString value )
		: this( new DerSequence( type, value ) ) {
	}

	public DomainDefinedAttribute( string type, string value )
		: this( new DerPrintableString( type ), new DerPrintableString( value ) ) {
	}
	public DomainDefinedAttribute( (string type, string value) x )
		: this( new DerPrintableString( x.type ), new DerPrintableString( x.value ) ) {
	}

	public DomainDefinedAttribute( DerPrintableString[] strs )
		: this( strs[0], strs[1] ) {
	}

	public DomainDefinedAttribute( string[] strs )
		: this( strs[0], strs[1] ) {
	}

	public virtual int Count => m_strings.Count;

	/**
	 * Return the PrintableString at index.
	 *
	 * @param index index of the string of interest
	 * @return the string at index.
	 */
	public DerPrintableString this[int index] => (DerPrintableString)m_strings[index];

	/**
	 * <pre>
	 * DomainDefinedAttribute ::= SEQUENCE SIZE (2) OF PrintableString
	 * </pre>
	 */
	public override Asn1Object ToAsn1Object() => m_strings;
}
