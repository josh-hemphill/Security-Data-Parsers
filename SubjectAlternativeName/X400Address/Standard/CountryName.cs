using Org.BouncyCastle.Asn1;

namespace SubjectAlternativeName.X400Address.Standard;

/*
CountryName ::= [APPLICATION 1] CHOICE {
	x121-dcc-code			NumericString		(SIZE (ub-country-name-numeric-length)),
	iso-3166-alpha2-code	PrintableString	(SIZE (ub-country-name-alpha-length))
}
*/
public class CountryName
			: Asn1Encodable, IAsn1Choice {
	public const int NumericLength = 3;
	public const int PrintableLength = 2;
	internal readonly IAsn1String contents;

	public CountryName( string text ) {
		contents = text.Length switch {
			NumericLength => new DerNumericString( text ),
			PrintableLength => new DerPrintableString( text ),
			_ => throw new ArgumentException( "unknown object in factory: CountryName(string text)" ),
		};
	}
	public CountryName( char[] text ) {
		contents = text.Length switch {
			NumericLength => new DerNumericString( new string( text ) ),
			PrintableLength => new DerPrintableString( new string( text ) ),
			_ => throw new ArgumentException( "unknown object in factory: CountryName( char[] text )" ),
		};
	}
	public CountryName( IAsn1String iaString ) {
		string text = iaString.GetString();
		contents = text.Length switch {
			NumericLength => new DerNumericString( text ),
			PrintableLength => new DerPrintableString( text ),
			_ => throw new ArgumentException( "unknown object in factory: CountryName( IAsn1String iaString )" ),
		};
	}

	public static CountryName GetInstance( object obj ) {
		return obj is IAsn1String asn1String
			? new CountryName( asn1String )
			: obj is CountryName countryName ?
				countryName :
				throw new ArgumentException( "unknown object in factory: CountryName.GetInstance( object obj )" );
	}

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
