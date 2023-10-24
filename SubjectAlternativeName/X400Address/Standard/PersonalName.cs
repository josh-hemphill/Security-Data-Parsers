using Org.BouncyCastle.Asn1;

namespace SubjectAlternativeName.X400Address.Standard;

/*
PersonalName ::= SET {
	surname					[0] IMPLICIT PrintableString	(SIZE (1..ub-surname-length)),
	given-name				[1] IMPLICIT PrintableString	(SIZE (1..ub-given-name-length))					OPTIONAL,
	initials					[2] IMPLICIT PrintableString	(SIZE (1..ub-initials-length))					OPTIONAL,
	generation-qualifier	[3] IMPLICIT PrintableString	(SIZE (1..ub-generation-qualifier-length))	OPTIONAL
}
-- see also teletex-personal-name

ub-surname-length						INTEGER ::= 40
ub-given-name-length					INTEGER ::= 16
ub-initials-length					INTEGER ::= 5
ub-generation-qualifier-length	INTEGER ::= 3
*/
public class PersonalName : Asn1Encodable {
	private readonly Asn1Set values;
	private DerPrintableString GetNTaggedValue( int n ) => DerPrintableString
		.GetInstance( values.Single( v => Asn1TaggedObject.GetInstance( values[0] ).TagNo == n ) );
	public virtual DerPrintableString Surname => GetNTaggedValue( 0 );
	public virtual DerPrintableString? GivenName => GetNTaggedValue( 1 );
	public virtual DerPrintableString? Initials => GetNTaggedValue( 2 );
	public virtual DerPrintableString? GenerationQualifier => GetNTaggedValue( 3 );

	private PersonalName( Asn1Set values ) {
		this.values = values;
	}

	public static PersonalName? GetInstance( object obj ) {
		return obj is PersonalName pn ?
			pn :
			null != obj ?
				new PersonalName( Asn1Set.GetInstance( obj ) ) :
				null;
	}

	/**
	 * Create a single valued PersonalName.
	 *
	 * @param value DerPrintableString value.
	 */
	public PersonalName( DerPrintableString value ) {
		values = new DerSet( value );
	}

	/**
	 * Create a multi-valued PersonalName.
	 *
	 * @param values DerPrintableString values making up the PersonalName
	 */
	public PersonalName( DerPrintableString[] values ) {
		this.values = new DerSet( values );
	}


	/**
	 * Return the number of DerPrintableString objects in this PersonalName,
	 *
	 * @return size of PersonalName, greater than 1 if multi-valued.
	 */
	public virtual int Count => values.Count;

	public virtual DerPrintableString? GetFirst() {
		return values.Count == 0 ?
			null :
			DerPrintableString.GetInstance( values[0] );
	}

	public virtual DerPrintableString[] GetTypesAndValues() {
		DerPrintableString[] tmp = new DerPrintableString[values.Count];

		for (int i = 0; i < tmp.Length; ++i) {
			tmp[i] = DerPrintableString.GetInstance( values[i] );
		}

		return tmp;
	}

	/**
	 * <pre>
	 * RelativeDistinguishedName ::=
	 *                     SET OF DerPrintableString

	 * DerPrintableString ::= SEQUENCE {
	 *        type     AttributeType,
	 *        value    AttributeValue }
	 * </pre>
	 * @return this object as its ASN1Primitive type
	 */
	public override Asn1Object ToAsn1Object() {
		return values;
	}
}
