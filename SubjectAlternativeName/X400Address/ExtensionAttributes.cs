using Org.BouncyCastle.Asn1;

namespace SecurityDataParsers.SubjectAlternativeName.X400Address;

/// <summary>
/// Represents a collection of ExtensionAttribute objects, which are used to store X.400 address extension attributes.
/// </summary>
/// <remarks>
/// <pre>
///   ExtensionAttributes ::= SET SIZE (1..ub-extension-attributes) OF ExtensionAttribute
///   ub-extension-attributes INTEGER ::= 256
/// </pre>
/// </remarks>
public class ExtensionAttributes
		: Asn1Encodable {
	/// <summary>
	/// Returns an ExtensionAttributes object representing the specified object.
	/// </summary>
	/// <param name="obj">The object to be converted to an ExtensionAttributes object.</param>
	/// <returns>An ExtensionAttributes object representing obj, or null if obj is null.</returns>
	public static ExtensionAttributes? GetInstance( object obj ) {
		return obj == null
			? null
			: obj is ExtensionAttributes extensionAttributes
				? extensionAttributes
				: new ExtensionAttributes( Asn1Sequence.GetInstance( obj ) );
	}

	/// <summary>
	/// Returns an ExtensionAttributes object representing the contents of the specified Asn1TaggedObject.
	/// </summary>
	/// <param name="taggedObject">The Asn1TaggedObject to be converted to an ExtensionAttributes object.</param>
	/// <param name="declaredExplicit">True if the tagged object is explicitly tagged, false otherwise.</param>
	/// <returns>An ExtensionAttributes object representing the contents of taggedObject.</returns>
	public static ExtensionAttributes GetInstance( Asn1TaggedObject taggedObject, bool declaredExplicit ) {
		return new ExtensionAttributes( Asn1Sequence.GetInstance( taggedObject, declaredExplicit ) );
	}

	private readonly Asn1Sequence m_obj;

	/// <summary>
	/// Initializes a new instance of the ExtensionAttributes class from the specified Asn1Sequence.
	/// </summary>
	/// <param name="seq">The Asn1Sequence to initialize the ExtensionAttributes object from.</param>
	internal ExtensionAttributes( Asn1Sequence seq ) {
		foreach (Asn1Encodable? element in seq) {
			if (element is not ExtensionAttribute)
				throw new ArgumentException( "attempt to insert non ExtensionAttribute into ExtensionAttributes" );
		}

		m_obj = seq;
	}

	/// <summary>
	/// Initializes a new instance of the ExtensionAttributes class from the specified ExtensionAttribute.
	/// </summary>
	/// <param name="p">The ExtensionAttribute to initialize the ExtensionAttributes object from.</param>
	public ExtensionAttributes( ExtensionAttribute p ) {
		m_obj = new DerSequence( p );
	}

	/// <summary>
	/// Initializes a new instance of the ExtensionAttributes class from the specified array of ExtensionAttribute objects.
	/// </summary>
	/// <param name="p">The array of ExtensionAttribute objects to initialize the ExtensionAttributes object from.</param>
	public ExtensionAttributes( ExtensionAttribute[] p ) {
		m_obj = new DerSequence( p );
	}

	/// <summary>
	/// Initializes a new instance of the ExtensionAttributes class from the specified type and value.
	/// </summary>
	/// <param name="type">The type of the extension attribute.</param>
	/// <param name="value">The value of the extension attribute.</param>
	public ExtensionAttributes( DerInteger type, Asn1Object value )
		: this( new ExtensionAttribute( type, value ) ) {
	}

	/// <summary>
	/// Initializes a new instance of the ExtensionAttributes class from the specified type and value.
	/// </summary>
	/// <param name="type">The type of the extension attribute.</param>
	/// <param name="value">The value of the extension attribute.</param>
	public ExtensionAttributes( int type, Asn1Object value )
		: this( new DerInteger( type ), value ) {
	}

	/// <summary>
	/// Initializes a new instance of the ExtensionAttributes class from the specified tuple of type and value.
	/// </summary>
	/// <param name="x">The tuple of type and value to initialize the ExtensionAttributes object from.</param>
	public ExtensionAttributes( (int type, Asn1Object value) x )
		: this( new DerInteger( x.type ), x.value ) {
	}

	/// <summary>
	/// Initializes a new instance of the ExtensionAttributes class from the specified type and value.
	/// </summary>
	/// <param name="type">The type of the extension attribute.</param>
	/// <param name="value">The value of the extension attribute.</param>
	public ExtensionAttributes( int type, byte[] value )
		: this( new DerInteger( type ), Asn1Object.FromByteArray( value ) ) {
	}

	/// <summary>
	/// Initializes a new instance of the ExtensionAttributes class from the specified tuple of type and value.
	/// </summary>
	/// <param name="x">The tuple of type and value to initialize the ExtensionAttributes object from.</param>
	public ExtensionAttributes( (int type, byte[] value) x )
		: this( new DerInteger( x.type ), Asn1Object.FromByteArray( x.value ) ) {
	}

	/// <summary>
	/// Initializes a new instance of the ExtensionAttributes class from the specified array of tuples of type and value.
	/// </summary>
	/// <param name="x">The array of tuples of type and value to initialize the ExtensionAttributes object from.</param>
	public ExtensionAttributes( (int type, byte[] value)[] x ) {
		m_obj = new DerSequence( x.Select( v => new ExtensionAttribute( v.type, v.value ) ).ToArray() );
	}


	/// <summary>
	/// Gets the number of ExtensionAttribute objects in the ExtensionAttributes object.
	/// </summary>
	public virtual int Count => m_obj.Count;

	/// <summary>
	/// Gets the ExtensionAttribute object at the specified index.
	/// </summary>
	/// <param name="index">The index of the ExtensionAttribute object to retrieve.</param>
	/// <returns>The ExtensionAttribute object at the specified index.</returns>
	public ExtensionAttribute this[int index] => (ExtensionAttribute)m_obj[index];

	/// <summary>
	/// Returns the Asn1Object representation of the ExtensionAttributes object.
	/// </summary>
	/// <remarks>
	/// <pre>
	/// ExtensionAttributes ::= SEQUENCE SIZE (1..4) OF BuiltInExtensionAttribute
	/// </pre>
	/// </remarks>
	/// <returns>The Asn1Object representation of the ExtensionAttributes object.</returns>
	public override Asn1Object ToAsn1Object() => m_obj;
}
