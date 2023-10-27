# DomainDefinedAttributes class

Represents a sequence of DomainDefinedAttribute objects.

```csharp
public class DomainDefinedAttributes : Asn1Encodable
```

## Public Members

| name | description |
| --- | --- |
| [DomainDefinedAttributes](DomainDefinedAttributes/DomainDefinedAttributes.md)(…) | Initializes a new instance of the [`DomainDefinedAttributes`](./DomainDefinedAttributes.md) class. (5 constructors) |
| static [GetInstance](DomainDefinedAttributes/GetInstance.md)(…) | Returns an instance of the [`DomainDefinedAttributes`](./DomainDefinedAttributes.md) class from an object. (2 methods) |
| virtual [Count](DomainDefinedAttributes/Count.md) { get; } | Gets the number of DomainDefinedAttribute objects in the sequence. |
| [Item](DomainDefinedAttributes/Item.md) { get; } | Gets the DomainDefinedAttribute object at the specified index. |
| override [ToAsn1Object](DomainDefinedAttributes/ToAsn1Object.md)() | Returns the ASN.1 object representation of the DomainDefinedAttributes object. |

## Remarks

BuiltInDomainDefinedAttributes ::= SEQUENCE SIZE (1..ub-domain-defined-attributes) OF BuiltInDomainDefinedAttribute ub-domain-defined-attributes INTEGER ::= 4

## See Also

* namespace [SecurityDataParsers.SubjectAlternativeName.X400Address](../SecurityDataParsers.SubjectAlternativeName.X400AddressNamespace.md.md)
* assembly [SubjectAlternativeName](../SubjectAlternativeName.md)
* [DomainDefinedAttributes.cs](https://github.com/josh-hemphill/Security-Data-Parsers/blob/master/SubjectAlternativeName/X400Address/DomainDefinedAttributes.cs)

<!-- DO NOT EDIT: generated by xmldocmd for SubjectAlternativeName.dll -->