using System.Formats.Asn1;
using Org.BouncyCastle.Asn1;
using SubjectAlternativeName.X400Address.Standard;
namespace SubjectAlternativeName.X400Address;

/*
BuiltInStandardAttributes ::= SEQUENCE {
	country-name						[APPLICATION 1]	CountryName								OPTIONAL,
	administration-domain-name		[APPLICATION 2]	AdministrationDomainName			OPTIONAL,
	network-address					[0]					IMPLICIT	NetworkAddress				OPTIONAL,
		--	see also extended-network-address
	terminal-identifier				[1]					IMPLICIT	TerminalIdentifier		OPTIONAL,
	private-domain-name				[2]					PrivateDomainName						OPTIONAL,
	organization-name					[3]					IMPLICIT	OrganizationName			OPTIONAL,
		--	see also teletex-organization-name
	numeric-user-identifier			[4]					IMPLICIT	NumericUserIdentifier	OPTIONAL,
	personal-name						[5]					IMPLICIT	PersonalName				OPTIONAL,
		--	see also teletex-personal-name
	organizational-unit-names		[6]					IMPLICIT	OrganizationalUnitNames	OPTIONAL
		--	see also teletex-organizational-unit-names
}

TerminalIdentifier		::= PrintableString	(SIZE (1..ub-terminal-id-length))
	ub-terminal-id-length		INTEGER ::= 24
NumericUserIdentifier	::= NumericString		(SIZE (1..ub-numeric-user-id-length))
	ub-numeric-user-id-length	INTEGER ::= 32
*/
public class StandardAttributes
		: Asn1Encodable {
	private readonly Asn1Sequence seq;
	public readonly CountryName? countryName;
	public readonly AdministrationDomainName? administrationDomainName;
	public readonly NetworkAddress? networkAddress;
	public readonly DerPrintableString? terminalIdentifier;
	public readonly PrivateDomainName? privateDomainName;
	public readonly OrganizationName? organizationName;
	public readonly DerPrintableString? numericUserIdentifier;
	public readonly PersonalName? personalName;
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

	public static StandardAttributes? GetInstance( object obj ) {
		return obj == null
			? null
			: obj is StandardAttributes standardAttributes
				? standardAttributes
				: new StandardAttributes( Asn1Sequence.GetInstance( obj ) );
	}

	public static StandardAttributes GetInstance( Asn1TaggedObject taggedObject, bool declaredExplicit ) {
		return new StandardAttributes( Asn1Sequence.GetInstance( taggedObject, declaredExplicit ) );
	}

	private static bool IsSameTag( Asn1TaggedObject x, Asn1Tag z ) => x.TagNo == z.TagValue && x.TagClass == (int)z.TagClass;
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
			else if (IsSameTag( tg, OrganizationalUnitNamesTag )) organizationalUnitNames = OrganizationalUnitNames.GetInstance( tg );
			else throw new ArgumentException( "unknown object in factory: PrivateDomainName( int type, string text )" );
		}
	}

	public StandardAttributes( Asn1Encodable[] strs )
		: this( new DerSequence( strs ) ) {
	}

	public override Asn1Object ToAsn1Object() => seq;
}
