using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X500;
namespace SecurityDataParsers.SubjectAlternativeName;

/*
EDIPartyName ::= SEQUENCE {
	nameAssigner	[0]	DirectoryString	OPTIONAL,
	partyName		[1]	DirectoryString
}
*/

/// <summary>
/// Represents an EDI party name.
/// </summary>
/// <remarks>
/// <pre>
/// EDIPartyName ::= SEQUENCE {
/// 	nameAssigner	[0]	DirectoryString	OPTIONAL,
/// 	partyName		[1]	DirectoryString
/// 		}
/// </pre>
/// </remarks>
public class EdiPartyName : Asn1Encodable {

	/// <summary>
	/// Gets an instance of EdiPartyName from the given object.
	/// </summary>
	/// <param name="obj">The object to get the instance from.</param>
	/// <returns>An instance of EdiPartyName.</returns>
	public static EdiPartyName? GetInstance( object obj ) {
		return obj == null
			 ? null
			 : obj is EdiPartyName ediPartyName ?
			  ediPartyName :
			  new EdiPartyName( Asn1Sequence.GetInstance( obj ) );
	}

	/// <summary>
	/// Gets an instance of EdiPartyName from the given tagged object.
	/// </summary>
	/// <param name="taggedObject">The tagged object to get the instance from.</param>
	/// <param name="declaredExplicit">Whether the object is declared explicit.</param>
	/// <returns>An instance of EdiPartyName.</returns>
	public static EdiPartyName? GetInstance( Asn1TaggedObject taggedObject, bool declaredExplicit ) {
		return GetInstance( Asn1Sequence.GetInstance( taggedObject, declaredExplicit ) );
	}

	private readonly DirectoryString? nameAssigner;
	private readonly DirectoryString partyName;

	/// <summary>
	/// Initializes a new instance of the EdiPartyName class.
	/// </summary>
	/// <param name="partyName">The party name.</param>
	/// <param name="nameAssigner">The name assigner.</param>
	public EdiPartyName( DirectoryString partyName, DirectoryString? nameAssigner ) {
		this.nameAssigner = nameAssigner;
		this.partyName = partyName;
	}

	private EdiPartyName( Asn1Sequence seq ) {
		nameAssigner = DirectoryString.GetInstance( seq[0] );
		partyName = DirectoryString.GetInstance( seq[1] );
	}

	/// <summary>
	/// Gets the party name.
	/// </summary>
	public virtual DirectoryString PartyName => partyName;

	/// <summary>
	/// Gets the name assigner.
	/// </summary>
	public virtual DirectoryString? NameAssigner => nameAssigner;

	/// <summary>
	/// Returns the ASN.1 object representation of this instance.
	/// </summary>
	/// <returns>The ASN.1 object representation of this instance.</returns>
	public override Asn1Object ToAsn1Object() {
		return new DerSequence( nameAssigner, partyName );
	}
}
