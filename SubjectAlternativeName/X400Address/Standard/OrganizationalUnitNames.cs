using Org.BouncyCastle.Asn1;

namespace SubjectAlternativeName.X400Address.Standard;

/*
OrganizationalUnitNames ::= SEQUENCE SIZE (1..ub-organizational-units) OF OrganizationalUnitName
OrganizationalUnitName ::= PrintableString (SIZE (1..ub-organizational-unit-name-length))

teletex-organizational-unit-names INTEGER ::= 5
TeletexOrganizationalUnitNames ::= SEQUENCE SIZE (1..ub-organizational-units) OF TeletexOrganizationalUnitName
TeletexOrganizationalUnitName ::= TeletexString (SIZE (1..ub-organizational-unit-name-length))


ub-organizational-units					INTEGER ::= 4
ub-organizational-unit-name-length	INTEGER ::= 32
*/
public class OrganizationalUnitNames
		: Asn1Encodable {
	public static OrganizationalUnitNames? GetInstance( object obj ) {
		return obj == null
			? null
			: obj is OrganizationalUnitNames organizationalUnitNames
				? organizationalUnitNames
				: new OrganizationalUnitNames( Asn1Sequence.GetInstance( obj ) );
	}

	public static OrganizationalUnitNames GetInstance( Asn1TaggedObject taggedObject, bool declaredExplicit ) {
		return new OrganizationalUnitNames( Asn1Sequence.GetInstance( taggedObject, declaredExplicit ) );
	}

	private readonly Asn1Sequence m_strings;

	internal OrganizationalUnitNames( Asn1Sequence seq ) {
		foreach (Asn1Encodable? element in seq) {
			if (element is not DerPrintableString)
				throw new ArgumentException( "attempt to insert non PrintableString into OrganizationalUnitNames" );
		}

		m_strings = seq;
	}

	public OrganizationalUnitNames( DerPrintableString p ) {
		m_strings = new DerSequence( p );
	}

	public OrganizationalUnitNames( string p )
		: this( new DerPrintableString( p ) ) {
	}

	public OrganizationalUnitNames( DerPrintableString[] strs ) {
		m_strings = new DerSequence( strs );
	}

	public OrganizationalUnitNames( string[] strs ) {
		Asn1EncodableVector v = new( strs.Length );
		for (int i = 0; i < strs.Length; i++) {
			v.Add( new DerPrintableString( strs[i] ) );
		}
		m_strings = new DerSequence( v );
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
	 * OrganizationalUnitNames ::= SEQUENCE SIZE (1..MAX) OF PrintableString
	 * </pre>
	 */
	public override Asn1Object ToAsn1Object() => m_strings;
}
