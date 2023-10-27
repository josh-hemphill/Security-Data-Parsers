using Org.BouncyCastle.Asn1;

namespace SecurityDataParsers.SubjectAlternativeName.X400Address.Standard;

/// <summary>
/// Represents a country name.
/// </summary>
/// <remarks>
/// <pre>
/// CountryName ::= [APPLICATION 1] CHOICE {
/// 	x121-dcc-code			NumericString		(SIZE (ub-country-name-numeric-length)),
/// 	is-3166-alpha2-code	PrintableString	(SIZE (ub-country-name-alpha-length))
/// }
/// ub-country-name-numeric-length INTEGER ::= 3
/// ub-country-name-alpha-length INTEGER ::= 2
/// </pre>
/// </remarks>
public class CountryName
			: Asn1Encodable, IAsn1Choice {
	/// <summary>
	/// The length of the numeric country code.
	/// </summary>
	public const int NumericLength = 3;

	/// <summary>
	/// The length of the printable country code.
	/// </summary>
	public const int PrintableLength = 2;

	internal readonly IAsn1String contents;

	/// <summary>
	/// Initializes a new instance of the <see cref="CountryName"/> class with the specified text.
	/// </summary>
	/// <param name="text">The text to use for the country name.</param>
	public CountryName( string text ) {
		contents = text.Length switch {
			NumericLength => new DerNumericString( text ),
			PrintableLength => new DerPrintableString( text ),
			_ => throw new ArgumentException( "unknown object in factory: CountryName(string text)" ),
		};
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CountryName"/> class with the specified character array.
	/// </summary>
	/// <param name="text">The character array to use for the country name.</param>
	public CountryName( char[] text ) {
		contents = text.Length switch {
			NumericLength => new DerNumericString( new string( text ) ),
			PrintableLength => new DerPrintableString( new string( text ) ),
			_ => throw new ArgumentException( "unknown object in factory: CountryName( char[] text )" ),
		};
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CountryName"/> class with the specified ASN.1 string.
	/// </summary>
	/// <param name="iaString">The ASN.1 string to use for the country name.</param>
	public CountryName( IAsn1String iaString ) {
		string text = iaString.GetString();
		contents = text.Length switch {
			NumericLength => new DerNumericString( text ),
			PrintableLength => new DerPrintableString( text ),
			_ => throw new ArgumentException( "unknown object in factory: CountryName( IAsn1String iaString )" ),
		};
	}

	/// <summary>
	/// Returns a new instance of the <see cref="CountryName"/> class from the specified object.
	/// </summary>
	/// <param name="obj">The object to create a new instance from.</param>
	/// <returns>A new instance of the <see cref="CountryName"/> class.</returns>
	public static CountryName GetInstance( object obj ) {
		return obj is IAsn1String asn1String
			? new CountryName( asn1String )
			: obj is CountryName countryName ?
				countryName :
				throw new ArgumentException( "unknown object in factory: CountryName.GetInstance( object obj )" );
	}

	/// <summary>
	/// Returns a new instance of the <see cref="CountryName"/> class from the specified tagged object.
	/// </summary>
	/// <param name="taggedObject">The tagged object to create a new instance from.</param>
	/// <param name="declaredExplicit">Whether the tagged object is declared explicit.</param>
	/// <returns>A new instance of the <see cref="CountryName"/> class.</returns>
	public static CountryName GetInstance( Asn1TaggedObject taggedObject, bool declaredExplicit ) {
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
	/// Gets the numeric country code.
	/// </summary>
	public virtual DerNumericString? X121Code => contents is DerNumericString x ? x : null;

	/// <summary>
	/// Gets the ISO 3166 alpha-2 country code.
	/// </summary>
	public virtual DerPrintableString? ISO3166Code => contents is DerPrintableString x ? x : null;

	/// <summary>
	/// Gets the country code.
	/// </summary>
	public virtual DerPrintableString? CountryCode => ISO3166Code;

	/// <inheritdoc/>
	public override Asn1Object ToAsn1Object() {
		return (Asn1Object)contents;
	}

	/// <summary>
	/// Gets the string representation of the country name.
	/// </summary>
	/// <returns>The string representation of the country name.</returns>
	public string GetString() {
		return contents.GetString();
	}
}
