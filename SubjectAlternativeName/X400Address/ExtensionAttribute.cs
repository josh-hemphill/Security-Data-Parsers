using Org.BouncyCastle.Asn1;

namespace SecurityDataParsers.SubjectAlternativeName.X400Address;

/// <summary>
/// Represents an ExtensionAttribute as defined in RFC 3280.
/// </summary>
/// <remarks>
/// <pre>
/// ExtensionAttribute ::=  SEQUENCE {
///    extension-attribute-type	[0] IMPLICIT INTEGER (0..ub-extension-attributes),
///    extension-attribute-value	[1] ANY DEFINED BY extension-attribute-type
/// }
/// ub-extension-attributes INTEGER ::= 256
/// </pre>
/// </remarks>
public class ExtensionAttribute
		: Asn1Encodable {

	/// <summary>
	/// Returns an instance of ExtensionAttribute from the given object.
	/// </summary>
	/// <param name="obj">The object to get the ExtensionAttribute instance from.</param>
	/// <returns>The ExtensionAttribute instance.</returns>
	/// <example>
	/// ExtensionAttribute extensionAttribute = ExtensionAttribute.GetInstance(obj);
	/// </example>
	public static ExtensionAttribute? GetInstance( object obj ) {
		return obj == null
			? null
			: obj is ExtensionAttribute extensionAttribute
				? extensionAttribute
				: new ExtensionAttribute( Asn1Sequence.GetInstance( obj ) );
	}

	/// <summary>
	/// Returns an instance of ExtensionAttribute from the given Asn1TaggedObject.
	/// </summary>
	/// <param name="taggedObject">The Asn1TaggedObject to get the ExtensionAttribute instance from.</param>
	/// <param name="declaredExplicit">Whether the tagged object is explicitly tagged.</param>
	/// <returns>The ExtensionAttribute instance.</returns>
	/// <example>
	/// ExtensionAttribute extensionAttribute = ExtensionAttribute.GetInstance(taggedObject, true);
	/// </example>
	public static ExtensionAttribute GetInstance( Asn1TaggedObject taggedObject, bool declaredExplicit ) {
		return new ExtensionAttribute( Asn1Sequence.GetInstance( taggedObject, declaredExplicit ) );
	}

	private readonly Asn1Sequence m_element;

	/// <summary>
	/// Gets the type of the ExtensionAttribute.
	/// </summary>
	/// <returns>The type of the ExtensionAttribute.</returns>
	/// <example>
	/// DerInteger type = extensionAttribute.Type;
	/// </example>
	public virtual DerInteger Type => DerInteger.GetInstance( m_element[0] );

	/// <summary>
	/// Gets the value of the ExtensionAttribute.
	/// </summary>
	/// <returns>The value of the ExtensionAttribute.</returns>
	/// <example>
	/// Asn1Object value = extensionAttribute.Value;
	/// </example>
	public virtual Asn1Object Value => m_element[1].ToAsn1Object();


	internal ExtensionAttribute( Asn1Sequence seq ) {
		if (seq.Count != 2)
			throw new ArgumentException( "incorrect number of arguments, ExtensionAttribute requires 2 strings (type, value)" );
		if (seq[0].ToAsn1Object() is not DerInteger)
			throw new ArgumentException( "attempt to insert non DerInteger into ExtensionAttribute" );

		m_element = seq;
	}

	/// <summary>
	/// Initializes a new instance of the ExtensionAttribute class with the given type and value.
	/// </summary>
	/// <param name="type">The type of the ExtensionAttribute.</param>
	/// <param name="value">The value of the ExtensionAttribute.</param>
	/// <example>
	/// ExtensionAttribute extensionAttribute = new ExtensionAttribute(new DerInteger(0), new DerPrintableString("value"));
	/// </example>
	public ExtensionAttribute( DerInteger type, Asn1Object value )
		: this( new DerSequence( type, value ) ) {
	}

	/// <summary>
	/// Initializes a new instance of the ExtensionAttribute class with the given type and value.
	/// </summary>
	/// <param name="type">The type of the ExtensionAttribute.</param>
	/// <param name="value">The value of the ExtensionAttribute.</param>
	/// <example>
	/// ExtensionAttribute extensionAttribute = new ExtensionAttribute(0, new DerPrintableString("value"));
	/// </example>
	public ExtensionAttribute( int type, Asn1Object value )
		: this( new DerInteger( type ), value ) {
	}

	/// <summary>
	/// Initializes a new instance of the ExtensionAttribute class with the given type and value.
	/// </summary>
	/// <param name="x">A tuple containing the type and value of the ExtensionAttribute.</param>
	/// <example>
	/// ExtensionAttribute extensionAttribute = new ExtensionAttribute((0, new DerPrintableString("value")));
	/// </example>
	public ExtensionAttribute( (int type, Asn1Object value) x )
		: this( new DerInteger( x.type ), x.value ) {
	}

	/// <summary>
	/// Initializes a new instance of the ExtensionAttribute class with the given type and value.
	/// </summary>
	/// <param name="type">The type of the ExtensionAttribute.</param>
	/// <param name="value">The value of the ExtensionAttribute.</param>
	/// <example>
	/// ExtensionAttribute extensionAttribute = new ExtensionAttribute(0, new byte[] { 0x01, 0x02, 0x03 });
	/// </example>
	public ExtensionAttribute( int type, byte[] value )
		: this( new DerInteger( type ), Asn1Object.FromByteArray( value ) ) {
	}

	/// <summary>
	/// Initializes a new instance of the ExtensionAttribute class with the given type and value.
	/// </summary>
	/// <param name="x">A tuple containing the type and value of the ExtensionAttribute.</param>
	/// <example>
	/// ExtensionAttribute extensionAttribute = new ExtensionAttribute((0, new byte[] { 0x01, 0x02, 0x03 }));
	/// </example>
	public ExtensionAttribute( (int type, byte[] value) x )
		: this( new DerInteger( x.type ), Asn1Object.FromByteArray( x.value ) ) {
	}

	/// <summary>
	/// Gets the number of elements in the ExtensionAttribute.
	/// </summary>
	/// <returns>The number of elements in the ExtensionAttribute.</returns>
	/// <example>
	/// int count = extensionAttribute.Count;
	/// </example>
	public virtual int Count => m_element.Count;

	/// <summary>
	/// Gets the PrintableString at the specified index.
	/// </summary>
	/// <param name="index">The index of the string to get.</param>
	/// <returns>The PrintableString at the specified index.</returns>
	/// <example>
	/// DerPrintableString printableString = extensionAttribute[0];
	/// </example>
	public DerPrintableString this[int index] => (DerPrintableString)m_element[index];

	/// <summary>
	/// Returns the ExtensionAttribute as an Asn1Object.
	/// </summary>
	/// <remarks>
	/// <pre>
	/// ExtensionAttribute ::= SEQUENCE SIZE (2)
	///   [0]: int extension-attribute-type
	///   [1]: variant extension-attribute-value
	/// </pre>
	/// </remarks>
	/// <returns>The ExtensionAttribute as an Asn1Object.</returns>
	/// <example>
	/// Asn1Object obj = extensionAttribute.ToAsn1Object();
	/// </example>
	public override Asn1Object ToAsn1Object() => m_element;
}
