using Org.BouncyCastle.Asn1;
namespace SecurityDataParsers.SubjectAlternativeName.X400Address.Standard;
/// <summary>
/// Represents an Administration Domain Name (ADMD) as defined in RFC 822.
/// </summary>
/// <remarks>
/// <pre>
/// AdministrationDomainName ::= [APPLICATION 2] CHOICE {
/// 	numeric		NumericString		(SIZE (0..ub-domain-name-length)),
/// 	printable	PrintableString	(SIZE (0..ub-domain-name-length))
/// }
/// ub-domain-name-length INTEGER ::= 16
/// </pre>
/// </remarks>
public class AdministrationDomainName
			: Asn1Encodable, IAsn1Choice {
	/// <summary>
	/// The maximum length of an Administration Domain Name.
	/// </summary>
	public const int AdministrationDomainNameMaxLength = 16;

	/// <summary>
	/// The content type for a numeric string.
	/// </summary>
	public const int ContentTypeNumericString = 0;

	/// <summary>
	/// The content type for a printable string.
	/// </summary>
	public const int ContentTypePrintableString = 1;

	private readonly int contentType;
	private readonly IAsn1String contents;

	/// <summary>
	/// Initializes a new instance of the <see cref="AdministrationDomainName"/> class with the specified content type and text.
	/// </summary>
	/// <param name="type">The content type of the ADMD.</param>
	/// <param name="text">The text to encapsulate. Strings longer than <see cref="AdministrationDomainNameMaxLength"/> are truncated.</param>
	public AdministrationDomainName(
			int type,
			string text ) {
		if (text.Length > AdministrationDomainNameMaxLength) {
			// RFC3280 limits these strings to 200 chars
			// truncate the string
			text = text[..AdministrationDomainNameMaxLength];
		}

		contentType = type;

		contents = type switch {
			ContentTypeNumericString => new DerNumericString( new string( text ) ),
			ContentTypePrintableString => new DerPrintableString( new string( text ) ),
			_ => throw new ArgumentException( "unknown object in factory: AdministrationDomainName( int type, string text )" ),
		};
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AdministrationDomainName"/> class with the specified text.
	/// </summary>
	/// <param name="text">The text to encapsulate. Strings longer than <see cref="AdministrationDomainNameMaxLength"/> are truncated.</param>
	public AdministrationDomainName(
		string text ) {
		// by default use PrintableString
		if (text.Length > AdministrationDomainNameMaxLength) {
			text = text[..AdministrationDomainNameMaxLength];
		}

		contentType = ContentTypePrintableString;
		contents = new DerPrintableString( text );
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AdministrationDomainName"/> class from its Asn1Encodable form.
	/// </summary>
	/// <remarks>
	/// <p>Useful when reading back a <code>AdministrationDomainName</code> class
	/// from it's Asn1Encodable form.</p>
	/// </remarks>
	/// <param name="contents">An <see cref="IAsn1String"/> instance.</param>
	public AdministrationDomainName(
		IAsn1String contents ) {
		this.contents = contents;
	}

	/// <summary>
	/// Gets an instance of <see cref="AdministrationDomainName"/> from the specified object.
	/// </summary>
	/// <param name="obj">The object to get an instance of <see cref="AdministrationDomainName"/> from.</param>
	/// <returns>An instance of <see cref="AdministrationDomainName"/>.</returns>
	public static AdministrationDomainName GetInstance( object obj ) {
		return obj is IAsn1String asn1String
			? new AdministrationDomainName( asn1String )
			: obj is AdministrationDomainName administrationDomainName ?
				administrationDomainName :
				throw new ArgumentException( "unknown object in factory: AdministrationDomainName.GetInstance( object obj )" );
	}

	/// <summary>
	/// Gets an instance of <see cref="AdministrationDomainName"/> from the specified tagged object.
	/// </summary>
	/// <param name="taggedObject">The tagged object to get an instance of <see cref="AdministrationDomainName"/> from.</param>
	/// <param name="declaredExplicit">Whether the tagged object is declared explicit.</param>
	/// <returns>An instance of <see cref="AdministrationDomainName"/>.</returns>
	public static AdministrationDomainName GetInstance( Asn1TaggedObject taggedObject, bool declaredExplicit ) {
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
	/// Gets the X.121 code for the ADMD.
	/// </summary>
	public virtual DerNumericString? X121Code => contents is DerNumericString x ? x : null;

	/// <summary>
	/// Gets the ISO 3166 code for the ADMD.
	/// </summary>
	public virtual DerPrintableString? ISO3166Code => contents is DerPrintableString x ? x : null;

	/// <summary>
	/// Gets the country code for the ADMD.
	/// </summary>
	public virtual DerPrintableString? CountryCode => ISO3166Code;

	/// <inheritdoc/>
	public override Asn1Object ToAsn1Object() {
		return (Asn1Object)contents;
	}

	/// <summary>
	/// Gets the string representation of the ADMD.
	/// </summary>
	/// <returns>The string representation of the ADMD.</returns>
	public string GetString() {
		return contents.GetString();
	}
}
