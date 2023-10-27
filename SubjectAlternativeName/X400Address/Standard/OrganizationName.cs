using Org.BouncyCastle.Asn1;

namespace SecurityDataParsers.SubjectAlternativeName.X400Address.Standard;


/// <summary>
/// Represents an X.400 organization name.
/// </summary>
/// <remarks>
/// <pre>
/// OrganizationName ::= PrintableString	(SIZE (1..ub-organization-name-length))
/// -- see also teletex-organization-name
///
/// ub-organization-name-length	INTEGER	::= 64
/// teletex-organization-name		INTEGER	::= 3
/// </pre>
/// </remarks>
public class OrganizationName
			: Asn1Encodable, IAsn1Choice {
	/// <summary>
	/// The maximum length of an organization name.
	/// </summary>
	public const int OrgMaxLength = 16;

	/// <summary>
	/// The maximum length of a Teletex organization name.
	/// </summary>
	public const int TeletexOrgMaxLength = 22;

	internal readonly DerPrintableString contents;

	/// <summary>
	/// Initializes a new instance of the <see cref="OrganizationName"/> class with the specified string.
	/// </summary>
	/// <param name="text">The string to use as the organization name.</param>
	public OrganizationName( string text ) {
		contents = new DerPrintableString( text );
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="OrganizationName"/> class with the specified character array.
	/// </summary>
	/// <param name="text">The character array to use as the organization name.</param>
	public OrganizationName( char[] text ) {
		contents = new DerPrintableString( new string( text ) );
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="OrganizationName"/> class with the specified <see cref="DerPrintableString"/>.
	/// </summary>
	/// <param name="iaString">The <see cref="DerPrintableString"/> to use as the organization name.</param>
	public OrganizationName( DerPrintableString iaString ) {
		contents = iaString;
	}

	/// <summary>
	/// Returns an <see cref="OrganizationName"/> object from the specified object.
	/// </summary>
	/// <param name="obj">The object to convert to an <see cref="OrganizationName"/> object.</param>
	/// <returns>An <see cref="OrganizationName"/> object.</returns>
	public static OrganizationName GetInstance( object obj ) {
		return obj is DerPrintableString asn1String
			? new OrganizationName( asn1String )
			: obj is OrganizationName organizationName ?
				organizationName :
				throw new ArgumentException( "unknown object in factory: OrganizationName.GetInstance( object obj )" );
	}

	/// <summary>
	/// Returns an <see cref="OrganizationName"/> object from the specified <see cref="Asn1TaggedObject"/>.
	/// </summary>
	/// <param name="taggedObject">The <see cref="Asn1TaggedObject"/> to convert to an <see cref="OrganizationName"/> object.</param>
	/// <param name="declaredExplicit">A boolean value indicating whether the object is explicitly tagged.</param>
	/// <returns>An <see cref="OrganizationName"/> object.</returns>
	public static OrganizationName GetInstance( Asn1TaggedObject taggedObject, bool declaredExplicit ) {
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
	/// Gets a value indicating whether the organization name can be used as an organization name.
	/// </summary>
	public virtual bool CanBeOrgName => contents.GetOctets().Length switch { >= 1 and <= OrgMaxLength => true, _ => false };

	/// <summary>
	/// Gets a value indicating whether the organization name can be used as a Teletex organization name.
	/// </summary>
	public virtual bool CanBeTeletex => contents.GetOctets().Length switch { >= 1 and <= TeletexOrgMaxLength => true, _ => false };

	/// <inheritdoc/>
	public override Asn1Object ToAsn1Object() {
		return contents;
	}

	/// <summary>
	/// Gets the string representation of the organization name.
	/// </summary>
	/// <returns>The string representation of the organization name.</returns>
	public string GetString() {
		return contents.GetString();
	}
}
