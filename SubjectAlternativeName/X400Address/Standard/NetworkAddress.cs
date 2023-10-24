using Org.BouncyCastle.Asn1;

namespace SubjectAlternativeName.X400Address.Standard;

/*
NetworkAddress ::= X121Address  -- see also extended-network-address

X121Address ::= NumericString (SIZE (1..ub-x121-address-length))

ub-x121-address-length INTEGER ::= 16
extended-network-address INTEGER ::= 22
*/

public class NetworkAddress
			: Asn1Encodable, IAsn1Choice {
	public const int AddressMaxLength = 16;
	public const int ExtendedMaxLength = 22;
	internal readonly DerNumericString contents;

	public NetworkAddress( string text ) {
		contents = new DerNumericString( text );
	}
	public NetworkAddress( char[] text ) {
		contents = new DerNumericString( new string( text ) );
	}
	public NetworkAddress( DerNumericString iaString ) {
		contents = iaString;
	}

	public static NetworkAddress GetInstance( object obj ) {
		return obj is DerNumericString asn1String
			? new NetworkAddress( asn1String )
			: obj is NetworkAddress NetworkAddress ?
				NetworkAddress :
				throw new ArgumentException( "unknown object in factory: NetworkAddress.GetInstance( object obj )" );
	}

	public static NetworkAddress GetInstance( Asn1TaggedObject taggedObject, bool declaredExplicit ) {
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


	public virtual bool CanBeBaseAddress => contents.GetOctets().Length switch { >= 1 and <= AddressMaxLength => true, _ => false };
	public virtual bool CanBeExtendedAddress => contents.GetOctets().Length switch { >= AddressMaxLength and <= ExtendedMaxLength => true, _ => false };

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
