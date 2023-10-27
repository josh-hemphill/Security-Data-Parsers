using Org.BouncyCastle.Asn1;
namespace SecurityDataParsers.SubjectAlternativeName.X400Address.Standard;
/// <summary>
/// Represents a network address.
/// </summary>
/// <remarks>
/// <pre>
/// NetworkAddress ::= X121Address  -- see also extended-network-address
/// X121Address ::= NumericString (SIZE (1..ub-x121-address-length))
///
/// ub-x121-address-length INTEGER ::= 16
/// extended-network-address INTEGER ::= 22
/// </pre>
/// </remarks>
public class NetworkAddress
			: Asn1Encodable, IAsn1Choice {
	/// <summary>
	/// The maximum length of the address.
	/// </summary>
	public const int AddressMaxLength = 16;

	/// <summary>
	/// The maximum length of the extended address.
	/// </summary>
	public const int ExtendedMaxLength = 22;

	internal readonly DerNumericString contents;

	/// <summary>
	/// Initializes a new instance of the <see cref="NetworkAddress"/> class with the specified text.
	/// </summary>
	/// <param name="text">The text to initialize the network address with.</param>
	public NetworkAddress( string text ) {
		contents = new DerNumericString( text );
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="NetworkAddress"/> class with the specified character array.
	/// </summary>
	/// <param name="text">The character array to initialize the network address with.</param>
	public NetworkAddress( char[] text ) {
		contents = new DerNumericString( new string( text ) );
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="NetworkAddress"/> class with the specified DER numeric string.
	/// </summary>
	/// <param name="iaString">The DER numeric string to initialize the network address with.</param>
	public NetworkAddress( DerNumericString iaString ) {
		contents = iaString;
	}

	/// <summary>
	/// Returns a <see cref="NetworkAddress"/> instance from the specified object.
	/// </summary>
	/// <param name="obj">The object to get the <see cref="NetworkAddress"/> instance from.</param>
	/// <returns>A <see cref="NetworkAddress"/> instance.</returns>
	public static NetworkAddress GetInstance( object obj ) {
		return obj is DerNumericString asn1String
			? new NetworkAddress( asn1String )
			: obj is NetworkAddress NetworkAddress ?
				NetworkAddress :
				throw new ArgumentException( "unknown object in factory: NetworkAddress.GetInstance( object obj )" );
	}

	/// <summary>
	/// Returns a <see cref="NetworkAddress"/> instance from the specified tagged object.
	/// </summary>
	/// <param name="taggedObject">The tagged object to get the <see cref="NetworkAddress"/> instance from.</param>
	/// <param name="declaredExplicit">A boolean value indicating whether the tagged object is declared explicitly.</param>
	/// <returns>A <see cref="NetworkAddress"/> instance.</returns>
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


	/// <summary>
	/// Gets a value indicating whether the network address can be a base address.
	/// </summary>
	public virtual bool CanBeBaseAddress => contents.GetOctets().Length switch { >= 1 and <= AddressMaxLength => true, _ => false };

	/// <summary>
	/// Gets a value indicating whether the network address can be an extended address.
	/// </summary>
	public virtual bool CanBeExtendedAddress => contents.GetOctets().Length switch { >= AddressMaxLength and <= ExtendedMaxLength => true, _ => false };

	/// <summary>
	/// Returns the ASN.1 object representation of the network address.
	/// </summary>
	/// <returns>The ASN.1 object representation of the network address.</returns>
	public override Asn1Object ToAsn1Object() {
		return contents;
	}

	/// <summary>
	/// Returns the string representation of the network address.
	/// </summary>
	/// <returns>The string representation of the network address.</returns>
	public string GetString() {
		return contents.GetString();
	}
}
