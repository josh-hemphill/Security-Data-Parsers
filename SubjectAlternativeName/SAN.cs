using Org.BouncyCastle.X509;
using FederalAgencySmartCredentialNumber;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.X509.Extension;
using System.Net;
using SubjectAlternativeName.X400Address;
using AsnUtils;

/*
	For info on the spec,
	See: https://datatracker.ietf.org/doc/html/rfc5280#section-4.2.1.6
*/


namespace SubjectAlternativeName;
public class SAN {

	public SAN( System.Security.Cryptography.X509Certificates.X509Certificate2 cert ) {
		ParseKnownFields(
			new X509CertificateParser()
			.ReadCertificate( cert.RawData )
			.GetSubjectAlternativeNameExtension() );
	}
	public SAN( System.Security.Cryptography.X509Certificates.X509SubjectAlternativeNameExtension SANExt ) : this( SANExt.RawData ) {
	}
	public SAN( byte[] x509Extension ) {
		Asn1OctetString altNames = Asn1OctetString.GetInstance( x509Extension );
		Asn1Object asn1Object = X509ExtensionUtilities.FromExtensionValue( altNames );
		ParseKnownFields( GeneralNames.GetInstance( asn1Object ) );
	}

	public OtherNames otherNames = new();
	public class OtherNames {
		public List<FASCN> FASCNs = new();
		public List<string> principalNames = new();
		public List<OtherName> unknown = new();
	}
	public List<string> rfc822Names = new();
	public List<DnsEndPoint> dnsNames = new();
	public List<ORAddress> x400Addresses = new();
	public List<X509Name> directoryNames = new();
	public List<EdiPartyName> ediPartyNames = new();
	public List<Uri> uniformResourceIdentifiers = new();
	public List<IPAddress> iPAddresses = new();
	public List<DerObjectIdentifier> registeredIDs = new();

	public void ParseKnownFields( GeneralNames SANExtNames ) {

		foreach (GeneralName? gn in SANExtNames.GetNames()) {
			switch (gn.TagNo) {
				case GeneralName.OtherName:
					OtherName otherName = OtherName.GetInstance( gn.Name );
					string lOid = otherName.TypeID.Id;
					if (lOid == CommonOids.FASCN.Id) {
						byte[] bits = new DerOctetString( otherName.Value ).GetOctets();
						otherNames.FASCNs.Add( new( bits ) );
					} else if (lOid == CommonOids.PrincipalName.Id) {
						otherNames.principalNames.Add( DerUtf8String.GetInstance( otherName.Value ).ToString() );
					}
					break;
				case GeneralName.DnsName:
					dnsNames.Add( new( ((IAsn1String)gn.Name).GetString(), 0 ) );
					break;
				case GeneralName.Rfc822Name:
					rfc822Names.Add( ((IAsn1String)gn.Name).GetString() );
					break;
				case GeneralName.X400Address:
					ORAddress? address = ORAddress.GetInstance( gn.Name );
					if (address != null) x400Addresses.Add( address );
					break;
				case GeneralName.DirectoryName:
					directoryNames.Add( X509Name.GetInstance( gn.Name ) );
					break;
				case GeneralName.EdiPartyName:
					EdiPartyName? edi = EdiPartyName.GetInstance( gn.Name );
					if (edi != null) ediPartyNames.Add( edi );
					break;
				case GeneralName.UniformResourceIdentifier:
					uniformResourceIdentifiers.Add( new( ((IAsn1String)gn.Name).GetString() ) );
					break;
				case GeneralName.IPAddress:
					byte[] addrBytes = Asn1OctetString.GetInstance( gn.Name ).GetOctets();
					IPAddress ipAddress = new( addrBytes );
					iPAddresses.Add( ipAddress );
					break;
				case GeneralName.RegisteredID:
					registeredIDs.Add( DerObjectIdentifier.GetInstance( gn.Name ) );
					break;
				default:
					throw new IOException( "Bad tag number: " + gn.TagNo );
			}
		}
	}

	public Firsts First => new( this );
	public class Firsts {
		private readonly SAN _SAN;
		internal Firsts( SAN s ) {
			_SAN = s;
		}
		public FASCN? FASCN => _SAN.otherNames.FASCNs.Count > 0 ? _SAN.otherNames.FASCNs.First() : null;
		public string? PrincipalName => _SAN.otherNames.principalNames.Count > 0 ? _SAN.otherNames.principalNames.First() : null;
		public string? Rfc822Name => _SAN.rfc822Names.Count > 0 ? _SAN.rfc822Names.First() : null;
		public DnsEndPoint? DnsName => _SAN.dnsNames.Count > 0 ? _SAN.dnsNames.First() : null;
		public ORAddress? X400Address => _SAN.x400Addresses.Count > 0 ? _SAN.x400Addresses.First() : null;
		public X509Name? DirectoryName => _SAN.directoryNames.Count > 0 ? _SAN.directoryNames.First() : null;
		public EdiPartyName? EdiPartyName => _SAN.ediPartyNames.Count > 0 ? _SAN.ediPartyNames.First() : null;
		public Uri? UniformResourceIdentifier => _SAN.uniformResourceIdentifiers.Count > 0 ? _SAN.uniformResourceIdentifiers.First() : null;
		public IPAddress? IPAddress => _SAN.iPAddresses.Count > 0 ? _SAN.iPAddresses.First() : null;
		public DerObjectIdentifier? RegisteredID => _SAN.registeredIDs.Count > 0 ? _SAN.registeredIDs.First() : null;

	}
}
