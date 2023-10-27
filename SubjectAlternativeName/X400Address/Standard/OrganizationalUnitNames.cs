using Org.BouncyCastle.Asn1;
namespace SecurityDataParsers.SubjectAlternativeName.X400Address.Standard;

/// <summary>
/// Represents a sequence of OrganizationalUnitName objects.
/// </summary>
/// <remarks>
/// <pre>
/// OrganizationalUnitNames ::= SEQUENCE SIZE (1..ub-organizational-units) OF OrganizationalUnitName
/// OrganizationalUnitName ::= PrintableString (SIZE (1..ub-organizational-unit-name-length))
///
/// teletex-organizational-unit-names INTEGER ::= 5
/// TeletexOrganizationalUnitNames ::= SEQUENCE SIZE (1..ub-organizational-units) OF TeletexOrganizationalUnitName
/// TeletexOrganizationalUnitName ::= TeletexString (SIZE (1..ub-organizational-unit-name-length))
///
///
/// ub-organizational-units					INTEGER ::= 4
/// ub-organizational-unit-name-length	INTEGER ::= 32
/// </pre>
/// </remarks>
public class OrganizationalUnitNames
		: Asn1Encodable {
	/// <summary>
	/// Initializes a new instance of the <see cref="OrganizationalUnitNames"/> class from an object.
	/// </summary>
	/// <param name="obj">The object to be converted.</param>
	/// <returns>A new instance of the <see cref="OrganizationalUnitNames"/> class.</returns>
	public static OrganizationalUnitNames? GetInstance( object obj ) {
		return obj == null
			? null
			: obj is OrganizationalUnitNames organizationalUnitNames
				? organizationalUnitNames
				: new OrganizationalUnitNames( Asn1Sequence.GetInstance( obj ) );
	}


	/// <summary>
	/// Initializes a new instance of the <see cref="OrganizationalUnitNames"/> class from a tagged object.
	/// </summary>
	/// <param name="taggedObject">The tagged object to be converted.</param>
	/// <param name="declaredExplicit">True if the object is explicitly tagged.</param>
	/// <returns>A new instance of the <see cref="OrganizationalUnitNames"/> class.</returns>
	public static OrganizationalUnitNames GetInstance( Asn1TaggedObject taggedObject, bool declaredExplicit ) {
		return new OrganizationalUnitNames( Asn1Sequence.GetInstance( taggedObject, declaredExplicit ) );
	}

	private readonly Asn1Sequence m_strings;

	/// <summary>
	/// Initializes a new instance of the <see cref="OrganizationalUnitNames"/> class from a sequence of printable strings.
	/// </summary>
	/// <param name="seq">The sequence of printable strings.</param>
	internal OrganizationalUnitNames( Asn1Sequence seq ) {
		foreach (Asn1Encodable? element in seq) {
			if (element is not DerPrintableString)
				throw new ArgumentException( "attempt to insert non PrintableString into OrganizationalUnitNames" );
		}

		m_strings = seq;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="OrganizationalUnitNames"/> class from a printable string.
	/// </summary>
	/// <param name="p">The printable string.</param>
	public OrganizationalUnitNames( DerPrintableString p ) {
		m_strings = new DerSequence( p );
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="OrganizationalUnitNames"/> class from a string.
	/// </summary>
	/// <param name="p">The string.</param>
	public OrganizationalUnitNames( string p )
		: this( new DerPrintableString( p ) ) {
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="OrganizationalUnitNames"/> class from an array of printable strings.
	/// </summary>
	/// <param name="strs">The array of printable strings.</param>
	public OrganizationalUnitNames( DerPrintableString[] strs ) {
		m_strings = new DerSequence( strs );
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="OrganizationalUnitNames"/> class from an array of strings.
	/// </summary>
	/// <param name="strs">The array of strings.</param>
	public OrganizationalUnitNames( string[] strs ) {
		Asn1EncodableVector v = new( strs.Length );
		for (int i = 0; i < strs.Length; i++) {
			v.Add( new DerPrintableString( strs[i] ) );
		}
		m_strings = new DerSequence( v );
	}

	/// <summary>
	/// Gets the number of elements in the sequence.
	/// </summary>
	public virtual int Count => m_strings.Count;

	/// <summary>
	/// Gets the printable string at the specified index.
	/// </summary>
	/// <param name="index">The index of the printable string to get.</param>
	/// <returns>The printable string at the specified index.</returns>
	public DerPrintableString this[int index] => (DerPrintableString)m_strings[index];

	/// <summary>
	/// Converts the object to its ASN.1 representation.
	/// </summary>
	/// <remarks>
	/// <pre>
	/// OrganizationalUnitNames ::= SEQUENCE SIZE (1..4) OF OrganizationalUnitName
	/// </pre>
	/// </remarks>
	/// <returns>The ASN.1 representation of the object.</returns>
	public override Asn1Object ToAsn1Object() => m_strings;
}
