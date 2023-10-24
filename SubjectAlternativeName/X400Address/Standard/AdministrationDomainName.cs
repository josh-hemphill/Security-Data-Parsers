using Org.BouncyCastle.Asn1;

namespace SubjectAlternativeName.X400Address.Standard;

/*
AdministrationDomainName ::= [APPLICATION 2] CHOICE {
	numeric		NumericString		(SIZE (0..ub-domain-name-length)),
	printable	PrintableString	(SIZE (0..ub-domain-name-length))
}
ub-domain-name-length INTEGER ::= 16
*/
public class AdministrationDomainName
			: Asn1Encodable, IAsn1Choice {
	public const int ContentTypeNumericString = 0;
	public const int ContentTypePrintableString = 1;
	public const int AdministrationDomainNameMaxLength = 16;
	internal readonly int contentType;
	internal readonly IAsn1String contents;

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
	/**
	 * Creates a new <code>AdministrationDomainName</code> instance.
	 *
	 * @param text the text to encapsulate. Strings longer than 200
	 * characters are truncated.
	 */
	public AdministrationDomainName(
		string text ) {
		// by default use PrintableString
		if (text.Length > AdministrationDomainNameMaxLength) {
			text = text[..AdministrationDomainNameMaxLength];
		}

		contentType = ContentTypePrintableString;
		contents = new DerPrintableString( text );
	}

	/**
	 * Creates a new <code>AdministrationDomainName</code> instance.
	 * <p>Useful when reading back a <code>AdministrationDomainName</code> class
	 * from it's Asn1Encodable form.</p>
	 *
	 * @param contents an <code>Asn1Encodable</code> instance.
	 */
	public AdministrationDomainName(
		IAsn1String contents ) {
		this.contents = contents;
	}

	public static AdministrationDomainName GetInstance( object obj ) {
		return obj is IAsn1String asn1String
			? new AdministrationDomainName( asn1String )
			: obj is AdministrationDomainName administrationDomainName ?
				administrationDomainName :
				throw new ArgumentException( "unknown object in factory: AdministrationDomainName.GetInstance( object obj )" );
	}

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


	public virtual DerNumericString? X121Code => contents is DerNumericString x ? x : null;
	public virtual DerPrintableString? ISO3166Code => contents is DerPrintableString x ? x : null;
	public virtual DerPrintableString? CountryCode => ISO3166Code;

	public override Asn1Object ToAsn1Object() {
		return (Asn1Object)contents;
	}

	/**
	 * Returns the stored <code>string</code> object.
	 *
	 * @return the stored text as a <code>string</code>.
	 */
	public string GetString() {
		return contents.GetString();
	}
}
