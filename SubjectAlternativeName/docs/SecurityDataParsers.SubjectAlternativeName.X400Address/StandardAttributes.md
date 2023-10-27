# StandardAttributes class

Represents the standard attributes of an X.400 address.

```csharp
public class StandardAttributes : Asn1Encodable
```

## Public Members

| name | description |
| --- | --- |
| [StandardAttributes](StandardAttributes/StandardAttributes.md)(…) | Initializes a new instance of the [`StandardAttributes`](./StandardAttributes.md) class. (2 constructors) |
| static [GetInstance](StandardAttributes/GetInstance.md)(…) | Gets an instance of [`StandardAttributes`](./StandardAttributes.md) from an object. (2 methods) |
| readonly [administrationDomainName](StandardAttributes/administrationDomainName.md) | Gets the administration domain name. |
| readonly [countryName](StandardAttributes/countryName.md) | Gets the country name. |
| readonly [networkAddress](StandardAttributes/networkAddress.md) | Gets the network address. |
| readonly [numericUserIdentifier](StandardAttributes/numericUserIdentifier.md) | Gets the numeric user identifier. |
| readonly [organizationalUnitNames](StandardAttributes/organizationalUnitNames.md) | Gets the organizational unit names. |
| readonly [organizationName](StandardAttributes/organizationName.md) | Gets the organization name. |
| readonly [personalName](StandardAttributes/personalName.md) | Gets the personal name. |
| readonly [privateDomainName](StandardAttributes/privateDomainName.md) | Gets the private domain name. |
| readonly [terminalIdentifier](StandardAttributes/terminalIdentifier.md) | Gets the terminal identifier. |
| override [ToAsn1Object](StandardAttributes/ToAsn1Object.md)() | Converts the [`StandardAttributes`](./StandardAttributes.md) to an ASN.1 object. |

## Remarks

BuiltInStandardAttributes ::= SEQUENCE { country-name [APPLICATION 1] CountryName OPTIONAL, administration-domain-name [APPLICATION 2] AdministrationDomainName OPTIONAL, network-address [0] IMPLICIT NetworkAddress OPTIONAL, -- see also extended-network-address terminal-identifier [1] IMPLICIT TerminalIdentifier OPTIONAL, private-domain-name [2] PrivateDomainName OPTIONAL, organization-name [3] IMPLICIT OrganizationName OPTIONAL, -- see also teletex-organization-name numeric-user-identifier [4] IMPLICIT NumericUserIdentifier OPTIONAL, personal-name [5] IMPLICIT PersonalName OPTIONAL, -- see also teletex-personal-name organizational-unit-names [6] IMPLICIT OrganizationalUnitNames OPTIONAL -- see also teletex-organizational-unit-names } TerminalIdentifier ::= PrintableString (SIZE (1..ub-terminal-id-length)) ub-terminal-id-length INTEGER ::= 24 NumericUserIdentifier ::= NumericString (SIZE (1..ub-numeric-user-id-length)) ub-numeric-user-id-length INTEGER ::= 32

## See Also

* namespace [SecurityDataParsers.SubjectAlternativeName.X400Address](../SecurityDataParsers.SubjectAlternativeName.X400AddressNamespace.md.md)
* assembly [SubjectAlternativeName](../SubjectAlternativeName.md)
* [StandardAttributes.cs](https://github.com/josh-hemphill/Security-Data-Parsers/blob/master/SubjectAlternativeName/X400Address/StandardAttributes.cs)

<!-- DO NOT EDIT: generated by xmldocmd for SubjectAlternativeName.dll -->