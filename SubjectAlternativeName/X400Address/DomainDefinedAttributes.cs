using Org.BouncyCastle.Asn1;

namespace SecurityDataParsers.SubjectAlternativeName.X400Address;

/// <summary>
/// Represents a sequence of DomainDefinedAttribute objects.
/// </summary>
/// <remarks>
/// <pre>
/// BuiltInDomainDefinedAttributes ::=
/// 	SEQUENCE SIZE (1..ub-domain-defined-attributes) OF BuiltInDomainDefinedAttribute
/// ub-domain-defined-attributes INTEGER ::= 4
/// </pre>
/// </remarks>
public class DomainDefinedAttributes
		: Asn1Encodable {

	/// <summary>
	/// Returns an instance of the <see cref="DomainDefinedAttributes"/> class from an object.
	/// </summary>
	/// <param name="obj">The object to get the instance from.</param>
	/// <returns>An instance of the <see cref="DomainDefinedAttributes"/> class.</returns>
	public static DomainDefinedAttributes? GetInstance( object obj ) {
		return obj == null
			? null
			: obj is DomainDefinedAttributes domainDefinedAttributes
				? domainDefinedAttributes
				: new DomainDefinedAttributes( Asn1Sequence.GetInstance( obj ) );
	}

	/// <summary>
	/// Returns an instance of the <see cref="DomainDefinedAttributes"/> class from a tagged object.
	/// </summary>
	/// <param name="taggedObject">The tagged object to get the instance from.</param>
	/// <param name="declaredExplicit">Whether the object is explicitly tagged.</param>
	/// <returns>An instance of the <see cref="DomainDefinedAttributes"/> class.</returns>
	public static DomainDefinedAttributes GetInstance( Asn1TaggedObject taggedObject, bool declaredExplicit ) {
		return new DomainDefinedAttributes( Asn1Sequence.GetInstance( taggedObject, declaredExplicit ) );
	}

	private readonly Asn1Sequence m_strings;

	/// <summary>
	/// Initializes a new instance of the <see cref="DomainDefinedAttributes"/> class.
	/// </summary>
	/// <param name="seq">The sequence of DomainDefinedAttribute objects.</param>
	internal DomainDefinedAttributes( Asn1Sequence seq ) {
		foreach (Asn1Encodable? element in seq) {
			if (element is not DomainDefinedAttribute)
				throw new ArgumentException( "attempt to insert non DomainDefinedAttribute into DomainDefinedAttributes" );
		}

		m_strings = seq;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DomainDefinedAttributes"/> class.
	/// </summary>
	/// <param name="p">The DomainDefinedAttribute object.</param>
	public DomainDefinedAttributes( DomainDefinedAttribute p ) {
		m_strings = new DerSequence( p );
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DomainDefinedAttributes"/> class.
	/// </summary>
	/// <param name="type">The type of the DomainDefinedAttribute.</param>
	/// <param name="value">The value of the DomainDefinedAttribute.</param>
	public DomainDefinedAttributes( string type, string value )
		: this( new DomainDefinedAttribute( type, value ) ) {
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DomainDefinedAttributes"/> class.
	/// </summary>
	/// <param name="x">A tuple containing the type and value of the DomainDefinedAttribute.</param>
	public DomainDefinedAttributes( (string type, string value) x )
		: this( x.type, x.value ) {
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DomainDefinedAttributes"/> class.
	/// </summary>
	/// <param name="strs">The array of DomainDefinedAttribute objects.</param>
	public DomainDefinedAttributes( DomainDefinedAttribute[] strs ) {
		m_strings = new DerSequence( strs );
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DomainDefinedAttributes"/> class.
	/// </summary>
	/// <param name="strs">The array of string arrays representing the type and value of the DomainDefinedAttribute objects.</param>
	public DomainDefinedAttributes( string[][] strs ) {
		Asn1EncodableVector v = new( strs.Length );
		for (int i = 0; i < strs.Length; i++) {
			v.Add( new DomainDefinedAttribute( strs[i] ) );
		}
		m_strings = new DerSequence( v );
	}


	/// <summary>
	/// Gets the number of DomainDefinedAttribute objects in the sequence.
	/// </summary>
	public virtual int Count => m_strings.Count;

	/// <summary>
	/// Gets the DomainDefinedAttribute object at the specified index.
	/// </summary>
	/// <param name="index">The index of the DomainDefinedAttribute object.</param>
	/// <returns>The DomainDefinedAttribute object at the specified index.</returns>
	public DomainDefinedAttribute this[int index] => (DomainDefinedAttribute)m_strings[index];

	/// <summary>
	/// Returns the ASN.1 object representation of the DomainDefinedAttributes object.
	/// </summary>
	/// <remarks>
	/// <pre>
	/// DomainDefinedAttributes ::= SEQUENCE SIZE (1..4) OF BuiltInDomainDefinedAttribute
	/// </pre>
	/// </remarks>
	/// <returns>The ASN.1 object representation of the DomainDefinedAttributes object.</returns>
	public override Asn1Object ToAsn1Object() => m_strings;
}
