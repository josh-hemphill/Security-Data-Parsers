using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X500;
namespace SubjectAlternativeName;

/*
EDIPartyName ::= SEQUENCE {
	nameAssigner	[0]	DirectoryString	OPTIONAL,
	partyName		[1]	DirectoryString
}
*/
public class EdiPartyName
			: Asn1Encodable {

	public static EdiPartyName? GetInstance( object obj ) {
		return obj == null
			 ? null
			 : obj is EdiPartyName ediPartyName ?
			  ediPartyName :
			  new EdiPartyName( Asn1Sequence.GetInstance( obj ) );
	}

	public static EdiPartyName? GetInstance( Asn1TaggedObject taggedObject, bool declaredExplicit ) {
		return GetInstance( Asn1Sequence.GetInstance( taggedObject, declaredExplicit ) );
	}
	private readonly DirectoryString? nameAssigner;
	private readonly DirectoryString partyName;
	/**
	 * Base constructor.
	 * @param typeID the type of the other name.
	 * @param value the ANY object that represents the value.
	 */
	public EdiPartyName( DirectoryString partyName, DirectoryString? nameAssigner ) {
		this.nameAssigner = nameAssigner;
		this.partyName = partyName;
	}

	private EdiPartyName( Asn1Sequence seq ) {
		nameAssigner = DirectoryString.GetInstance( seq[0] );
		partyName = DirectoryString.GetInstance( seq[1] );
	}

	public virtual DirectoryString PartyName => partyName;

	public virtual DirectoryString? NameAssigner => nameAssigner;

	public override Asn1Object ToAsn1Object() {
		return new DerSequence( nameAssigner, partyName );
	}
}
