using Org.BouncyCastle.Asn1;

namespace SubjectAlternativeName.X400Address;

/*
ExtensionAttributes ::= SET SIZE (1..ub-extension-attributes) OF ExtensionAttribute
ub-extension-attributes INTEGER ::= 256
*/
public class ExtensionAttributes
		: Asn1Encodable {
	public static ExtensionAttributes? GetInstance( object obj ) {
		return obj == null
			? null
			: obj is ExtensionAttributes extensionAttributes
				? extensionAttributes
				: new ExtensionAttributes( Asn1Sequence.GetInstance( obj ) );
	}

	public static ExtensionAttributes GetInstance( Asn1TaggedObject taggedObject, bool declaredExplicit ) {
		return new ExtensionAttributes( Asn1Sequence.GetInstance( taggedObject, declaredExplicit ) );
	}

	private readonly Asn1Sequence m_obj;

	internal ExtensionAttributes( Asn1Sequence seq ) {
		foreach (Asn1Encodable? element in seq) {
			if (element is not ExtensionAttribute)
				throw new ArgumentException( "attempt to insert non ExtensionAttribute into ExtensionAttributes" );
		}

		m_obj = seq;
	}

	public ExtensionAttributes( ExtensionAttribute p ) {
		m_obj = new DerSequence( p );
	}
	public ExtensionAttributes( ExtensionAttribute[] p ) {
		m_obj = new DerSequence( p );
	}

	public ExtensionAttributes( DerInteger type, Asn1Object value )
		: this( new ExtensionAttribute( type, value ) ) {
	}
	public ExtensionAttributes( int type, Asn1Object value )
		: this( new DerInteger( type ), value ) {
	}
	public ExtensionAttributes( (int type, Asn1Object value) x )
		: this( new DerInteger( x.type ), x.value ) {
	}
	public ExtensionAttributes( int type, byte[] value )
		: this( new DerInteger( type ), Asn1Object.FromByteArray( value ) ) {
	}
	public ExtensionAttributes( (int type, byte[] value) x )
		: this( new DerInteger( x.type ), Asn1Object.FromByteArray( x.value ) ) {
	}
	public ExtensionAttributes( (int type, byte[] value)[] x ) {
		m_obj = new DerSequence( x.Select( v => new ExtensionAttribute( v.type, v.value ) ).ToArray() );
	}


	public virtual int Count => m_obj.Count;

	/**
	 * Return the ExtensionAttribute at index.
	 *
	 * @param index index of the ExtensionAttribute of interest
	 * @return the ExtensionAttribute at index.
	 */
	public ExtensionAttribute this[int index] => (ExtensionAttribute)m_obj[index];

	/**
	 * <pre>
	 * ExtensionAttributes ::= SEQUENCE SIZE (1..4) OF BuiltInExtensionAttribute
	 * </pre>
	 */
	public override Asn1Object ToAsn1Object() => m_obj;
}
