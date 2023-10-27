using Org.BouncyCastle.Asn1;

namespace SecurityDataParsers.SubjectAlternativeName.X400Address.Standard;

/// <summary>
/// Represents a private domain name.
/// </summary>
/// <remarks>
/// <pre>
/// PrivateDomainName ::= CHOICE {
/// 	numeric		NumericString		(SIZE (1..ub-domain-name-length)),
/// 	printable	PrintableString	(SIZE (1..ub-domain-name-length))
/// }
/// ub-domain-name-length INTEGER ::= 16
/// </pre>
/// </remarks>
public class PrivateDomainName
			: Asn1Encodable, IAsn1Choice {
	/// <summary>
	/// The content type for numeric strings.
	/// </summary>
	public const int ContentTypeNumericString = 0;

	/// <summary>
	/// The content type for printable strings.
	/// </summary>
	public const int ContentTypePrintableString = 1;

	/// <summary>
	/// The maximum length of a private domain name.
	/// </summary>
	public const int PrivateDomainNameMaxLength = 16;

	private readonly int contentType;
	private readonly IAsn1String contents;

	/// <summary>
	/// Initializes a new instance of the <see cref="PrivateDomainName"/> class with the specified content type and text.
	/// </summary>
	/// <param name="type">The content type.</param>
	/// <param name="text">The text to encapsulate. Strings longer than 200 characters are truncated.</param>
	public PrivateDomainName( int type, string text ) {
		if (text.Length > PrivateDomainNameMaxLength) {
			// RFC3280 limits these strings to 200 chars
			// truncate the string
			text = text[..PrivateDomainNameMaxLength];
		}

		contentType = type;

		contents = type switch {
			ContentTypeNumericString => new DerNumericString( new string( text ) ),
			ContentTypePrintableString => new DerPrintableString( new string( text ) ),
			_ => throw new ArgumentException( "unknown object in factory: PrivateDomainName( int type, string text )" ),
		};
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="PrivateDomainName"/> class with the specified text.
	/// </summary>
	/// <param name="text">The text to encapsulate. Strings longer than 200 characters are truncated.</param>
	public PrivateDomainName( string text ) {
		// by default use PrintableString
		if (text.Length > PrivateDomainNameMaxLength) {
			text = text[..PrivateDomainNameMaxLength];
		}

		contentType = ContentTypePrintableString;
		contents = new DerPrintableString( text );
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="PrivateDomainName"/> class from its Asn1Encodable form.
	/// </summary>
	/// <param name="contents">An <see cref="IAsn1String"/> instance.</param>
	public PrivateDomainName( IAsn1String contents ) {
		this.contents = contents;
	}

	/// <summary>
	/// Gets a <see cref="PrivateDomainName"/> instance from the specified object.
	/// </summary>
	/// <param name="obj">The object to get the instance from.</param>
	/// <returns>A <see cref="PrivateDomainName"/> instance.</returns>
	public static PrivateDomainName GetInstance( object obj ) {
		return obj is IAsn1String asn1String
			? new PrivateDomainName( asn1String )
			: obj is PrivateDomainName privateDomainName ?
				privateDomainName :
				throw new ArgumentException( "unknown object in factory: PrivateDomainName.GetInstance( object obj )" );
	}

	/// <summary>
	/// Gets a <see cref="PrivateDomainName"/> instance from the specified tagged object.
	/// </summary>
	/// <param name="taggedObject">The tagged object to get the instance from.</param>
	/// <param name="declaredExplicit">True if the object is explicitly tagged, false otherwise.</param>
	/// <returns>A <see cref="PrivateDomainName"/> instance.</returns>
	public static PrivateDomainName GetInstance( Asn1TaggedObject taggedObject, bool declaredExplicit ) {
		return GetInstanceFromChoice( taggedObject, declaredExplicit, GetInstance );
	}

	internal static TChoice GetInstanceFromChoice<TChoice>(
		Asn1TaggedObject taggedObject,
		bool declaredExplicit,
		Func<object, TChoice> constructor
	) where TChoice : Asn1Encodable, IAsn1Choice {
		return !declaredExplicit
				? throw new ArgumentException( $"Implicit tagging cannot be used with untagged choice type {typeof( TChoice ).GetType().FullName} (X.680 30.6, 30.8).", nameof( declaredExplicit ) )
				: taggedObject == null
				? throw new ArgumentNullException( nameof( taggedObject ) )
				: constructor( taggedObject.GetExplicitBaseObject() );
	}

	/// <summary>
	/// Gets the X.121 code for this private domain name.
	/// </summary>
	public virtual DerNumericString? X121Code => contents is DerNumericString x ? x : null;

	/// <summary>
	/// Gets the ISO 3166 code for this private domain name.
	/// </summary>
	public virtual DerPrintableString? ISO3166Code => contents is DerPrintableString x ? x : null;

	/// <summary>
	/// Gets the country code for this private domain name.
	/// </summary>
	public virtual DerPrintableString? CountryCode => ISO3166Code;

	/// <inheritdoc/>
	public override Asn1Object ToAsn1Object() {
		return (Asn1Object)contents;
	}

	/// <summary>
	/// Gets the stored text as a <see cref="string"/>.
	/// </summary>
	/// <returns>The stored text as a <see cref="string"/>.</returns>
	public string GetString() {
		return contents.GetString();
	}
}
