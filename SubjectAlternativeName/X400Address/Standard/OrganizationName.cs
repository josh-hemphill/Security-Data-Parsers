using Org.BouncyCastle.Asn1;

namespace SubjectAlternativeName.X400Address.Standard;

/*
OrganizationName ::= PrintableString	(SIZE (1..ub-organization-name-length))
-- see also teletex-organization-name

ub-organization-name-length	INTEGER	::= 64
teletex-organization-name		INTEGER	::= 3
*/

public class OrganizationName
			: Asn1Encodable, IAsn1Choice {
	public const int OrgMaxLength = 16;
	public const int TeletexOrgMaxLength = 22;
	internal readonly DerPrintableString contents;

	public OrganizationName( string text ) {
		contents = new DerPrintableString( text );
	}
	public OrganizationName( char[] text ) {
		contents = new DerPrintableString( new string( text ) );
	}
	public OrganizationName( DerPrintableString iaString ) {
		contents = iaString;
	}

	public static OrganizationName GetInstance( object obj ) {
		return obj is DerPrintableString asn1String
			? new OrganizationName( asn1String )
			: obj is OrganizationName organizationName ?
				organizationName :
				throw new ArgumentException( "unknown object in factory: OrganizationName.GetInstance( object obj )" );
	}

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


	public virtual bool CanBeOrgName => contents.GetOctets().Length switch { >= 1 and <= OrgMaxLength => true, _ => false };
	public virtual bool CanBeTeletex => contents.GetOctets().Length switch { >= 1 and <= TeletexOrgMaxLength => true, _ => false };

	public override Asn1Object ToAsn1Object() {
		return contents;
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
