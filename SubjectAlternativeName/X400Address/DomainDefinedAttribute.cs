using Org.BouncyCastle.Asn1;

namespace SecurityDataParsers.SubjectAlternativeName.X400Address;

/// <summary>
/// DomainDefinedAttribute ::= SEQUENCE SIZE (2) OF PrintableString
/// </summary>
/// <remarks>
/// <pre>
/// BuiltInDomainDefinedAttribute ::= SEQUENCE {
/// 	type PrintableString (SIZE (1..ub-domain-defined-attribute-type-length)),
/// 	value PrintableString (SIZE (1..ub-domain-defined-attribute-value-length))
/// }
/// ub-domain-defined-attribute-type-length INTEGER ::= 8
/// ub-domain-defined-attribute-value-length INTEGER ::= 128
/// </pre>
/// </remarks>
public class DomainDefinedAttribute
		: Asn1Encodable {
	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <param name="obj">The object.</param>
	/// <returns></returns>
	public static DomainDefinedAttribute? GetInstance( object obj ) {
		return obj == null
			? null
			: obj is DomainDefinedAttribute domainDefinedAttribute
				? domainDefinedAttribute
				: new DomainDefinedAttribute( Asn1Sequence.GetInstance( obj ) );
	}

	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <param name="taggedObject">The tagged object.</param>
	/// <param name="declaredExplicit">if set to <c>true</c> [declared explicit].</param>
	/// <returns></returns>
	public static DomainDefinedAttribute GetInstance( Asn1TaggedObject taggedObject, bool declaredExplicit ) {
		return new DomainDefinedAttribute( Asn1Sequence.GetInstance( taggedObject, declaredExplicit ) );
	}

	private readonly Asn1Sequence m_strings;

	/// <summary>
	/// Gets the type.
	/// </summary>
	public virtual DerPrintableString Type => DerPrintableString.GetInstance( m_strings[0] );

	/// <summary>
	/// Gets the value.
	/// </summary>
	public virtual DerPrintableString Value => DerPrintableString.GetInstance( m_strings[1] );


	/// <summary>
	/// Initializes a new instance of the <see cref="DomainDefinedAttribute"/> class.
	/// </summary>
	/// <param name="seq">The sequence.</param>
	internal DomainDefinedAttribute( Asn1Sequence seq ) {
		if (seq.Count != 2) throw new ArgumentException( "incorrect number of arguments, DomainDefinedAttribute requires 2 strings (type, value)" );
		foreach (Asn1Encodable? element in seq) {
			if (element is not DerPrintableString)
				throw new ArgumentException( "attempt to insert non PrintableString into DomainDefinedAttribute" );
		}

		m_strings = seq;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DomainDefinedAttribute"/> class.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="value">The value.</param>
	public DomainDefinedAttribute( DerPrintableString type, DerPrintableString value )
		: this( new DerSequence( type, value ) ) {
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DomainDefinedAttribute"/> class.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="value">The value.</param>
	public DomainDefinedAttribute( string type, string value )
		: this( new DerPrintableString( type ), new DerPrintableString( value ) ) {
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DomainDefinedAttribute"/> class.
	/// </summary>
	/// <param name="x">The x.</param>
	public DomainDefinedAttribute( (string type, string value) x )
		: this( new DerPrintableString( x.type ), new DerPrintableString( x.value ) ) {
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DomainDefinedAttribute"/> class.
	/// </summary>
	/// <param name="strs">The strings.</param>
	public DomainDefinedAttribute( DerPrintableString[] strs )
		: this( strs[0], strs[1] ) {
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DomainDefinedAttribute"/> class.
	/// </summary>
	/// <param name="strs">The strings.</param>
	public DomainDefinedAttribute( string[] strs )
		: this( strs[0], strs[1] ) {
	}


	/// <summary>
	/// Gets the count.
	/// </summary>
	public virtual int Count => m_strings.Count;


	/// <summary>
	/// Gets the <see cref="DerPrintableString"/> at the specified index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>The <see cref="DerPrintableString"/> at the specified index.</returns>
	public DerPrintableString this[int index] => (DerPrintableString)m_strings[index];

	/// <summary>
	/// To the ASN1 object.
	/// </summary>
	/// <remarks>
	/// <pre>
	/// DomainDefinedAttribute ::= SEQUENCE SIZE (2) OF PrintableString
	/// </pre>
	/// </remarks>
	/// <returns></returns>
	public override Asn1Object ToAsn1Object() => m_strings;
}
