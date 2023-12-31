# ExtensionAttribute constructor (1 of 5)

Initializes a new instance of the ExtensionAttribute class with the given type and value.

```csharp
public ExtensionAttribute((int type, Asn1Object value) x)
```

| parameter | description |
| --- | --- |
| x | A tuple containing the type and value of the ExtensionAttribute. |

## Examples

ExtensionAttribute extensionAttribute = new ExtensionAttribute((0, new DerPrintableString("value")));

## See Also

* class [ExtensionAttribute](../ExtensionAttributeType)
* namespace [SecurityDataParsers.SubjectAlternativeName.X400Address](../ExtensionAttributeType)
* assembly [SubjectAlternativeName](../../SubjectAlternativeNameAssembly)

---

# ExtensionAttribute constructor (2 of 5)

Initializes a new instance of the ExtensionAttribute class with the given type and value.

```csharp
public ExtensionAttribute((int type, byte[] value) x)
```

| parameter | description |
| --- | --- |
| x | A tuple containing the type and value of the ExtensionAttribute. |

## Examples

ExtensionAttribute extensionAttribute = new ExtensionAttribute((0, new byte[] { 0x01, 0x02, 0x03 }));

## See Also

* class [ExtensionAttribute](../ExtensionAttributeType)
* namespace [SecurityDataParsers.SubjectAlternativeName.X400Address](../ExtensionAttributeType)
* assembly [SubjectAlternativeName](../../SubjectAlternativeNameAssembly)

---

# ExtensionAttribute constructor (3 of 5)

Initializes a new instance of the ExtensionAttribute class with the given type and value.

```csharp
public ExtensionAttribute(DerInteger type, Asn1Object value)
```

| parameter | description |
| --- | --- |
| type | The type of the ExtensionAttribute. |
| value | The value of the ExtensionAttribute. |

## Examples

ExtensionAttribute extensionAttribute = new ExtensionAttribute(new DerInteger(0), new DerPrintableString("value"));

## See Also

* class [ExtensionAttribute](../ExtensionAttributeType)
* namespace [SecurityDataParsers.SubjectAlternativeName.X400Address](../ExtensionAttributeType)
* assembly [SubjectAlternativeName](../../SubjectAlternativeNameAssembly)

---

# ExtensionAttribute constructor (4 of 5)

Initializes a new instance of the ExtensionAttribute class with the given type and value.

```csharp
public ExtensionAttribute(int type, Asn1Object value)
```

| parameter | description |
| --- | --- |
| type | The type of the ExtensionAttribute. |
| value | The value of the ExtensionAttribute. |

## Examples

ExtensionAttribute extensionAttribute = new ExtensionAttribute(0, new DerPrintableString("value"));

## See Also

* class [ExtensionAttribute](../ExtensionAttributeType)
* namespace [SecurityDataParsers.SubjectAlternativeName.X400Address](../ExtensionAttributeType)
* assembly [SubjectAlternativeName](../../SubjectAlternativeNameAssembly)

---

# ExtensionAttribute constructor (5 of 5)

Initializes a new instance of the ExtensionAttribute class with the given type and value.

```csharp
public ExtensionAttribute(int type, byte[] value)
```

| parameter | description |
| --- | --- |
| type | The type of the ExtensionAttribute. |
| value | The value of the ExtensionAttribute. |

## Examples

ExtensionAttribute extensionAttribute = new ExtensionAttribute(0, new byte[] { 0x01, 0x02, 0x03 });

## See Also

* class [ExtensionAttribute](../ExtensionAttributeType)
* namespace [SecurityDataParsers.SubjectAlternativeName.X400Address](../ExtensionAttributeType)
* assembly [SubjectAlternativeName](../../SubjectAlternativeNameAssembly)

<!-- DO NOT EDIT: generated by xmldocmd for SubjectAlternativeName.dll -->
