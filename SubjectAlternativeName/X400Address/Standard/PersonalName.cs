using Org.BouncyCastle.Asn1;

namespace SecurityDataParsers.SubjectAlternativeName.X400Address.Standard;
/// <summary>
/// Represents a PersonalName object in X.400 address format.
/// </summary>
/// <remarks>
/// <pre>
/// PersonalName ::= SET {
/// 	surname					[0] IMPLICIT PrintableString	(SIZE (1..ub-surname-length)),
/// 	given-name				[1] IMPLICIT PrintableString	(SIZE (1..ub-given-name-length))					OPTIONAL,
/// 	initials					[2] IMPLICIT PrintableString	(SIZE (1..ub-initials-length))					OPTIONAL,
/// 	generation-qualifier	[3] IMPLICIT PrintableString	(SIZE (1..ub-generation-qualifier-length))	OPTIONAL
/// }
/// -- see also teletex-personal-name
///
/// ub-surname-length						INTEGER ::= 40
/// ub-given-name-length					INTEGER ::= 16
/// ub-initials-length					INTEGER ::= 5
/// ub-generation-qualifier-length	INTEGER ::= 3
/// </pre>
/// </remarks>
public class PersonalName : Asn1Encodable {
	private readonly Asn1Set values;
	private DerPrintableString GetNTaggedValue( int n ) => DerPrintableString
		.GetInstance( values.Single( v => Asn1TaggedObject.GetInstance( values[0] ).TagNo == n ) );

	/// <summary>
	/// Gets the Surname value of the PersonalName object.
	/// </summary>
	public virtual DerPrintableString Surname => GetNTaggedValue( 0 );

	/// <summary>
	/// Gets the GivenName value of the PersonalName object.
	/// </summary>
	public virtual DerPrintableString? GivenName => GetNTaggedValue( 1 );

	/// <summary>
	/// Gets the Initials value of the PersonalName object.
	/// </summary>
	public virtual DerPrintableString? Initials => GetNTaggedValue( 2 );

	/// <summary>
	/// Gets the GenerationQualifier value of the PersonalName object.
	/// </summary>
	public virtual DerPrintableString? GenerationQualifier => GetNTaggedValue( 3 );

	private PersonalName( Asn1Set values ) {
		this.values = values;
	}

	/// <summary>
	/// Returns a PersonalName object from the given object.
	/// </summary>
	/// <param name="obj">The object to be converted to a PersonalName object.</param>
	/// <returns>The PersonalName object.</returns>
	public static PersonalName? GetInstance( object obj ) {
		return obj is PersonalName pn ?
			pn :
			null != obj ?
				new PersonalName( Asn1Set.GetInstance( obj ) ) :
				null;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="PersonalName"/> class with a single value.
	/// </summary>
	/// <param name="value">The value to be added to the PersonalName object.</param>
	public PersonalName( DerPrintableString value ) {
		values = new DerSet( value );
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="PersonalName"/> class with an array of values.
	/// </summary>
	/// <param name="values">The values to be added to the PersonalName object.</param>
	public PersonalName( DerPrintableString[] values ) {
		this.values = new DerSet( values );
	}

	/// <summary>
	/// Gets the first DerPrintableString object in the PersonalName object.
	/// </summary>
	/// <returns>The first DerPrintableString object in the PersonalName object.</returns>
	public virtual DerPrintableString? GetFirst() {
		return values.Count == 0 ?
			null :
			DerPrintableString.GetInstance( values[0] );
	}

	/// <summary>
	/// Gets an array of DerPrintableString objects representing the types and values of the PersonalName object.
	/// </summary>
	/// <returns>An array of DerPrintableString objects representing the types and values of the PersonalName object.</returns>
	public virtual DerPrintableString[] GetTypesAndValues() {
		DerPrintableString[] tmp = new DerPrintableString[values.Count];

		for (int i = 0; i < tmp.Length; ++i) {
			tmp[i] = DerPrintableString.GetInstance( values[i] );
		}

		return tmp;
	}

	/// <summary>
	/// Gets the number of DerPrintableString objects in the PersonalName object.
	/// </summary>
	public virtual int Count => values.Count;

	/// <summary>
	/// Gets the DerPrintableString object at the specified index.
	/// </summary>
	/// <remarks>
	/// <pre>
	/// RelativeDistinguishedName ::=
	///                     SET OF DerPrintableString
	/// DerPrintableString ::= SEQUENCE {
	///        type     AttributeType,
	///        value    AttributeValue }
	/// </pre>
	/// </remarks>
	public override Asn1Object ToAsn1Object() {
		return values;
	}
}
