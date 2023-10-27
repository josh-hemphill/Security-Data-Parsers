using System.Formats.Asn1;
using Org.BouncyCastle.Asn1;
using SecurityDataParsers.SubjectAlternativeName.X400Address.Standard;
namespace SecurityDataParsers.SubjectAlternativeName.X400Address;

/// <summary>
///  Represents the standard attributes of an X.400 address.
/// </summary>
/// <remarks>
/// <pre>
/// 	BuiltInStandardAttributes ::= SEQUENCE {
/// 		country-name						[APPLICATION 1]	CountryName								OPTIONAL,
/// 		administration-domain-name		[APPLICATION 2]	AdministrationDomainName			OPTIONAL,
/// 		network-address					[0]					IMPLICIT	NetworkAddress				OPTIONAL,
/// 			--	see also extended-network-address
/// 		terminal-identifier				[1]					IMPLICIT	TerminalIdentifier		OPTIONAL,
/// 		private-domain-name				[2]					PrivateDomainName						OPTIONAL,
/// 		organization-name					[3]					IMPLICIT	OrganizationName			OPTIONAL,
/// 			--	see also teletex-organization-name
/// 		numeric-user-identifier			[4]					IMPLICIT	NumericUserIdentifier	OPTIONAL,
/// 		personal-name						[5]					IMPLICIT	PersonalName				OPTIONAL,
/// 			--	see also teletex-personal-name
/// 		organizational-unit-names		[6]					IMPLICIT	OrganizationalUnitNames	OPTIONAL
/// 			--	see also teletex-organizational-unit-names
/// 	}
/// 	TerminalIdentifier		::= PrintableString	(SIZE (1..ub-terminal-id-length))
/// 		ub-terminal-id-length		INTEGER ::= 24
/// 	NumericUserIdentifier	::= NumericString		(SIZE (1..ub-numeric-user-id-length))
/// 		ub-numeric-user-id-length	INTEGER ::= 32
/// </pre>
/// </remarks>
public class StandardAttributes : Asn1Encodable {
	private readonly Asn1Sequence seq;

	///  <summary>
	///  Gets the country name.
	///  </summary>
	public readonly CountryName? countryName;

	///  <summary>
	///  Gets the administration domain name.
	///  </summary>
	public readonly AdministrationDomainName? administrationDomainName;

	///  <summary>
	///  Gets the network address.
	///  </summary>
	public readonly NetworkAddress? networkAddress;

	///  <summary>
	///  Gets the terminal identifier.
	///  </summary>
	public readonly DerPrintableString? terminalIdentifier;

	///  <summary>
	///  Gets the private domain name.
	///  </summary>
	public readonly PrivateDomainName? privateDomainName;

	///  <summary>
	///  Gets the organization name.
	///  </summary>
	public readonly OrganizationName? organizationName;

	///  <summary>
	///  Gets the numeric user identifier.
	///  </summary>
	public readonly DerPrintableString? numericUserIdentifier;

	///  <summary>
	///  Gets the personal name.
	///  </summary>
	public readonly PersonalName? personalName;

	///  <summary>
	///  Gets the organizational unit names.
	///  </summary>
	public readonly OrganizationalUnitNames? organizationalUnitNames;

	private static Asn1Tag CountryNameTag =>
		new( TagClass.Application, 0, true );

	private static Asn1Tag AdministrationDomainNameTag =>
		new( TagClass.Application, 1, true );

	private static Asn1Tag NetworkAddressTag =>
		new( TagClass.Universal, 0, true );

	private static Asn1Tag TerminalIdentifierTag =>
		new( TagClass.Universal, 1, true );

	private static Asn1Tag PrivateDomainNameTag =>
		new( TagClass.Universal, 2, true );

	private static Asn1Tag OrganizationNameTag =>
		new( TagClass.Universal, 3, true );

	private static Asn1Tag NumericUserIdentifierTag =>
		new( TagClass.Universal, 4, true );

	private static Asn1Tag PersonalNameTag =>
		new( TagClass.Universal, 5, true );

	private static Asn1Tag OrganizationalUnitNamesTag =>
		new( TagClass.Universal, 6, true );

	///  <summary>
	///  Gets an instance of <see cref="StandardAttributes"/> from an object.
	///  </summary>
	///  <param name="obj">The object to get the instance from.</param>
	///  <returns>An instance of <see cref="StandardAttributes"/>.</returns>
	public static StandardAttributes? GetInstance( object obj ) {
		return obj == null
			? null
			: obj is StandardAttributes standardAttributes
				? standardAttributes
				: new StandardAttributes( Asn1Sequence.GetInstance( obj ) );
	}

	///  <summary>
	///  Gets an instance of <see cref="StandardAttributes"/> from a tagged object.
	///  </summary>
	///  <param name="taggedObject">The tagged object to get the instance from.</param>
	///  <param name="declaredExplicit">Whether the tagged object is declared explicit.</param>
	///  <returns>An instance of <see cref="StandardAttributes"/>.</returns>
	public static StandardAttributes GetInstance( Asn1TaggedObject taggedObject, bool declaredExplicit ) {
		return new StandardAttributes( Asn1Sequence.GetInstance( taggedObject, declaredExplicit ) );
	}

	private static bool IsSameTag( Asn1TaggedObject x, Asn1Tag z ) => x.TagNo == z.TagValue && x.TagClass == (int)z.TagClass;

	///  <summary>
	///  Initializes a new instance of the <see cref="StandardAttributes"/> class.
	///  </summary>
	///  <param name="seq">The ASN.1 sequence.</param>
	public StandardAttributes( Asn1Sequence seq ) {
		this.seq = seq;
		foreach (Asn1Encodable? element in seq) {
			Asn1TaggedObject tg = Asn1TaggedObject.GetInstance( element );
			if (IsSameTag( tg, CountryNameTag )) countryName = CountryName.GetInstance( tg );
			else if (IsSameTag( tg, AdministrationDomainNameTag )) administrationDomainName = AdministrationDomainName.GetInstance( tg );
			else if (IsSameTag( tg, NetworkAddressTag )) networkAddress = NetworkAddress.GetInstance( tg );
			else if (IsSameTag( tg, TerminalIdentifierTag )) terminalIdentifier = DerPrintableString.GetInstance( tg );
			else if (IsSameTag( tg, PrivateDomainNameTag )) privateDomainName = PrivateDomainName.GetInstance( tg );
			else if (IsSameTag( tg, OrganizationNameTag )) organizationName = OrganizationName.GetInstance( tg );
			else if (IsSameTag( tg, NumericUserIdentifierTag )) numericUserIdentifier = DerPrintableString.GetInstance( tg );
			else if (IsSameTag( tg, PersonalNameTag )) personalName = PersonalName.GetInstance( tg );
			else organizationalUnitNames = IsSameTag( tg, OrganizationalUnitNamesTag )
			  ? OrganizationalUnitNames.GetInstance( tg )
			  : throw new ArgumentException( "unknown object in factory: PrivateDomainName( int type, string text )" );
		}
	}

	///  <summary>
	///  Initializes a new instance of the <see cref="StandardAttributes"/> class.
	///  </summary>
	///  <param name="strs">The ASN.1 encodable classes.</param>
	public StandardAttributes( Asn1Encodable[] strs )
		: this( new DerSequence( strs ) ) {
	}

	///  <summary>
	///  Converts the <see cref="StandardAttributes"/> to an ASN.1 object.
	///  </summary>
	///  <returns>The ASN.1 object.</returns>
	public override Asn1Object ToAsn1Object() => seq;
}
