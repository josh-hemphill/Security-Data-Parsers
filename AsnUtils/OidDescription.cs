

using System.Security.Cryptography;
namespace AsnUtils;

internal readonly partial struct OidDescription : IEquatable<OidDescription> {
	public required string Id { get; init; }
	public required string FriendlyName { get; init; }
	public required string ShortName { get; init; }
	public required string Abbreviation { get; init; }

	public OidDescription( Oid oid ) {
		Id = oid.Value ?? "";
		FriendlyName = oid.FriendlyName ?? "";
	}

	public Oid Oid => new( Id, FriendlyName );

	public (string?, string?) OidTuple => (Id, FriendlyName);

	public static (string?, string?) OidToTuple( Oid x ) {
		return (x.Value, x.FriendlyName);
	}
	public override bool Equals( object? obj ) => (obj is OidDescription od && Equals( od )) ||
		(obj is Oid o && Equals( o ));

	public bool Equals( Oid? lOid ) => lOid != null && OidTuple.Equals( OidToTuple( lOid ) );

	public bool Equals( OidDescription oid ) => OidTuple.Equals( oid.OidTuple );

	public override int GetHashCode() => OidTuple.GetHashCode();

	public static bool operator ==( OidDescription lhs, OidDescription rhs ) {
		return lhs.Equals( rhs );
	}

	public static bool operator !=( OidDescription lhs, OidDescription rhs ) {
		return !(lhs == rhs);
	}
}
