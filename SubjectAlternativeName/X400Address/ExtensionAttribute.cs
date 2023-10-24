using Org.BouncyCastle.Asn1;

namespace SubjectAlternativeName.X400Address;

/*
ExtensionAttribute ::=  SEQUENCE {
   extension-attribute-type	[0] IMPLICIT INTEGER (0..ub-extension-attributes),
   extension-attribute-value	[1] ANY DEFINED BY extension-attribute-type
}
ub-extension-attributes INTEGER ::= 256
*/
public class ExtensionAttribute
		: Asn1Encodable {
	public static ExtensionAttribute? GetInstance( object obj ) {
		return obj == null
			? null
			: obj is ExtensionAttribute extensionAttribute
				? extensionAttribute
				: new ExtensionAttribute( Asn1Sequence.GetInstance( obj ) );
	}

	public static ExtensionAttribute GetInstance( Asn1TaggedObject taggedObject, bool declaredExplicit ) {
		return new ExtensionAttribute( Asn1Sequence.GetInstance( taggedObject, declaredExplicit ) );
	}

	private readonly Asn1Sequence m_element;

	public virtual DerInteger Type => DerInteger.GetInstance( m_element[0] );
	public virtual Asn1Object Value => m_element[1].ToAsn1Object();


	internal ExtensionAttribute( Asn1Sequence seq ) {
		if (seq.Count != 2)
			throw new ArgumentException( "incorrect number of arguments, ExtensionAttribute requires 2 strings (type, value)" );
		if (seq[0].ToAsn1Object() is not DerInteger)
			throw new ArgumentException( "attempt to insert non DerInteger into ExtensionAttribute" );

		m_element = seq;
	}

	public ExtensionAttribute( DerInteger type, Asn1Object value )
		: this( new DerSequence( type, value ) ) {
	}

	public ExtensionAttribute( int type, Asn1Object value )
		: this( new DerInteger( type ), value ) {
	}
	public ExtensionAttribute( (int type, Asn1Object value) x )
		: this( new DerInteger( x.type ), x.value ) {
	}
	public ExtensionAttribute( int type, byte[] value )
		: this( new DerInteger( type ), Asn1Object.FromByteArray( value ) ) {
	}
	public ExtensionAttribute( (int type, byte[] value) x )
		: this( new DerInteger( x.type ), Asn1Object.FromByteArray( x.value ) ) {
	}

	public virtual int Count => m_element.Count;

	/**
	 * Return the PrintableString at index.
	 *
	 * @param index index of the string of interest
	 * @return the string at index.
	 */
	public DerPrintableString this[int index] => (DerPrintableString)m_element[index];

	/**
	 * <pre>
	 * ExtensionAttribute ::= SEQUENCE SIZE (2)
	 *   [0]: int extension-attribute-type
	 *   [1]: variant extension-attribute-value
	 * </pre>
	 */
	public override Asn1Object ToAsn1Object() => m_element;
}
