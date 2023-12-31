# CountryName class

Represents a country name.

```csharp
public class CountryName : Asn1Encodable, IAsn1Choice
```

## Public Members

| name | description |
| --- | --- |
| [CountryName](CountryName/CountryName)(…) | Initializes a new instance of the [`CountryName`](./CountryNameType) class with the specified text. (3 constructors) |
| static [GetInstance](CountryName/GetInstance)(…) | Returns a new instance of the [`CountryName`](./CountryNameType) class from the specified object. (2 methods) |
| virtual [CountryCode](CountryName/CountryCode) { get; } | Gets the country code. |
| virtual [ISO3166Code](CountryName/ISO3166Code) { get; } | Gets the ISO 3166 alpha-2 country code. |
| virtual [X121Code](CountryName/X121Code) { get; } | Gets the numeric country code. |
| [GetString](CountryName/GetString)() | Gets the string representation of the country name. |
| override [ToAsn1Object](CountryName/ToAsn1Object)() |  |
| const [NumericLength](CountryName/NumericLength) | The length of the numeric country code. |
| const [PrintableLength](CountryName/PrintableLength) | The length of the printable country code. |

## Remarks

CountryName ::= [APPLICATION 1] CHOICE { x121-dcc-code NumericString (SIZE (ub-country-name-numeric-length)), is-3166-alpha2-code PrintableString (SIZE (ub-country-name-alpha-length)) } ub-country-name-numeric-length INTEGER ::= 3 ub-country-name-alpha-length INTEGER ::= 2

## See Also

* namespace [SecurityDataParsers.SubjectAlternativeName.X400Address.Standard](../SecurityDataParsersSubjectAlternativeNameX400AddressStandardNamespace)
* assembly [SubjectAlternativeName](../SubjectAlternativeNameAssembly)
* [CountryName.cs](https://github.com/josh-hemphill/Security-Data-Parsers/tree/latest/SubjectAlternativeName/X400Address/Standard/CountryName.cs)

<!-- DO NOT EDIT: generated by xmldocmd for SubjectAlternativeName.dll -->
