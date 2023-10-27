using System.Formats.Asn1;
using Org.BouncyCastle.Asn1;

namespace SecurityDataParsers.SubjectAlternativeName.X400Address;

/// <summary>
/// Represents an ORAddress object, which is defined in RFC 5280
/// </summary>
/// <remarks>
/// <pre>
/// ORAddress ::= SEQUENCE {
///     built-in-standard-attributes         BuiltInStandardAttributes,
///     built-in-domain-defined-attributes   BuiltInDomainDefinedAttributes OPTIONAL,
///     -- see also teletex-domain-defined-attributes
///     extension-attributes                 ExtensionAttributes OPTIONAL
/// }
/// </pre>
/// </remarks>
public class ORAddress
		: Asn1Encodable {
	private readonly Asn1Sequence seq;

	/// <summary>
	/// Gets the standard attributes of the ORAddress.
	/// </summary>
	public readonly StandardAttributes? standardAttributes;

	/// <summary>
	/// Gets the domain-defined attributes of the ORAddress.
	/// </summary>
	public readonly DomainDefinedAttributes? domainDefinedAttributes;

	/// <summary>
	/// Gets the extension attributes of the ORAddress.
	/// </summary>
	public readonly ExtensionAttributes? extensionAttributes;

	/// <summary>
	/// Initializes a new instance of the ORAddress class from an Asn1Sequence object.
	/// </summary>
	/// <param name="seq">The Asn1Sequence object to create the ORAddress from.</param>
	public ORAddress( Asn1Sequence seq ) {
		this.seq = seq;
		standardAttributes = StandardAttributes.GetInstance( seq[0] );
		if (seq[1] != null) domainDefinedAttributes = DomainDefinedAttributes.GetInstance( seq[1] );
		if (seq[2] != null) extensionAttributes = ExtensionAttributes.GetInstance( seq[2] );
	}

	/// <summary>
	/// Initializes a new instance of the ORAddress class from an array of Asn1Encodable objects.
	/// </summary>
	/// <param name="strs">The array of Asn1Encodable objects to create the ORAddress from.</param>
	public ORAddress( Asn1Encodable[] strs )
		: this( new DerSequence( strs ) ) {
	}

	/// <summary>
	/// Gets an instance of the ORAddress class from an object.
	/// </summary>
	/// <param name="obj">The object to create the ORAddress from.</param>
	/// <returns>An instance of the ORAddress class.</returns>
	public static ORAddress? GetInstance( object obj ) {
		return obj == null
			? null
			: obj is ORAddress oRAddress
				? oRAddress
				: new ORAddress( Asn1Sequence.GetInstance( obj ) );
	}

	/// <summary>
	/// Gets an instance of the ORAddress class from a tagged object.
	/// </summary>
	/// <param name="taggedObject">The tagged object to create the ORAddress from.</param>
	/// <param name="declaredExplicit">Whether the tagged object is declared explicit.</param>
	/// <returns>An instance of the ORAddress class.</returns>
	public static ORAddress GetInstance( Asn1TaggedObject taggedObject, bool declaredExplicit ) {
		return new ORAddress( Asn1Sequence.GetInstance( taggedObject, declaredExplicit ) );
	}

	/// <summary>
	/// Determines whether the specified tagged object has the same tag as the specified tag.
	/// </summary>
	/// <param name="x">The tagged object to compare.</param>
	/// <param name="z">The tag to compare.</param>
	/// <returns>True if the tagged object has the same tag as the specified tag, false otherwise.</returns>
	private static bool IsSameTag( Asn1TaggedObject x, Asn1Tag z ) => x.TagNo == z.TagValue && x.TagClass == (int)z.TagClass;

	/// <summary>
	/// Returns the ASN.1 object representing the ORAddress.
	/// </summary>
	/// <returns>The ASN.1 object representing the ORAddress.</returns>
	public override Asn1Object ToAsn1Object() => seq;
}
