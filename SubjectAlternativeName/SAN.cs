using Org.BouncyCastle.X509;
using SecurityDataParsers.FederalAgencySmartCredentialNumber;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.X509.Extension;
using System.Net;
using SecurityDataParsers.SubjectAlternativeName.X400Address;
using AsnUtils;

/*
	For info on the spec,
	See: https://datatracker.ietf.org/doc/html/rfc5280#section-4.2.1.6
*/


namespace SecurityDataParsers.SubjectAlternativeName;

/// <summary>
/// Represents the Subject Alternative Name (SAN) extension of an X.509 certificate.
/// For info on the spec, see: <see href="https://datatracker.ietf.org/doc/html/rfc5280#section-4.2.1.6">RFC 5280:4.2.1.6</see>
/// </summary>
public class SAN {

	/// <summary>
	/// Initializes a new instance of the <see cref="SAN"/> class with the specified X.509 certificate.
	/// </summary>
	/// <param name="cert">The X.509 certificate.</param>
	public SAN( System.Security.Cryptography.X509Certificates.X509Certificate2 cert ) {
		ParseKnownFields(
			new X509CertificateParser()
			.ReadCertificate( cert.RawData )
			.GetSubjectAlternativeNameExtension() );
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SAN"/> class with the specified X.509 Subject Alternative Name extension.
	/// </summary>
	/// <param name="SANExt">The X.509 Subject Alternative Name extension.</param>
	public SAN( System.Security.Cryptography.X509Certificates.X509SubjectAlternativeNameExtension SANExt ) : this( SANExt.RawData ) {
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="SAN"/> class with the specified X.509 extension value.
	/// </summary>
	/// <param name="x509Extension">The X.509 extension value.</param>
	public SAN( byte[] x509Extension ) {
		Asn1OctetString altNames = Asn1OctetString.GetInstance( x509Extension );
		Asn1Object asn1Object = X509ExtensionUtilities.FromExtensionValue( altNames );
		ParseKnownFields( GeneralNames.GetInstance( asn1Object ) );
	}

	/// <summary>
	/// Gets or sets the other names in the SAN extension.
	/// </summary>
	public OtherNames otherNames = new();
	/// <summary>
	/// Represents the OtherName records in the SAN extension of an X.509 certificate.
	/// </summary>
	public class OtherNames {
		/// <summary>
		/// Gets or sets the Federal Agency Smart Credential Number (FASCN) values in the SAN extension.
		/// </summary>
		public List<FASCN> FASCNs = new();

		/// <summary>
		/// Gets or sets the principal names in the SAN extension.
		/// </summary>
		public List<string> principalNames = new();

		/// <summary>
		/// Gets or sets the unknown names in the SAN extension.
		/// </summary>
		public List<OtherName> unknown = new();
	}

	/// <summary>
	/// Gets or sets the RFC 822 names in the SAN extension.
	/// </summary>
	public List<string> rfc822Names = new();

	/// <summary>
	/// Gets or sets the DNS names in the SAN extension.
	/// </summary>
	public List<DnsEndPoint> dnsNames = new();

	/// <summary>
	/// Gets or sets the X.400 addresses in the SAN extension.
	/// </summary>
	public List<ORAddress> x400Addresses = new();

	/// <summary>
	/// Gets or sets the directory names in the SAN extension.
	/// </summary>
	public List<X509Name> directoryNames = new();

	/// <summary>
	/// Gets or sets the EDI party names in the SAN extension.
	/// </summary>
	public List<EdiPartyName> ediPartyNames = new();

	/// <summary>
	/// Gets or sets the uniform resource identifiers in the SAN extension.
	/// </summary>
	public List<Uri> uniformResourceIdentifiers = new();

	/// <summary>
	/// Gets or sets the IP addresses in the SAN extension.
	/// </summary>
	public List<IPAddress> iPAddresses = new();

	/// <summary>
	/// Gets or sets the registered IDs in the SAN extension.
	/// </summary>
	public List<DerObjectIdentifier> registeredIDs = new();

	/// <summary>
	/// Parses the known fields in the SAN extension.
	/// </summary>
	/// <param name="SANExtNames">The GeneralNames object that contains the SAN extension values.</param>
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
	/// <summary>
	/// Provides access to the first value of each type of name in the SAN extension.
	/// </summary>
	public Firsts First => new( this );
	/// <summary>
	/// Provides access to the first value of each type of name in the SAN extension.
	/// </summary>
	public record Firsts {
		private readonly SAN _SAN;
		internal Firsts( SAN s ) {
			_SAN = s;
		}

		/// <summary>
		/// Gets the first Federal Agency Smart Credential Number (FASCN) value in the SAN extension.
		/// </summary>
		public FASCN? FASCN => _SAN.otherNames.FASCNs.Count > 0 ? _SAN.otherNames.FASCNs.First() : null;

		/// <summary>
		/// Gets the first principal name in the SAN extension.
		/// </summary>
		public string? PrincipalName => _SAN.otherNames.principalNames.Count > 0 ? _SAN.otherNames.principalNames.First() : null;

		/// <summary>
		/// Gets the first RFC 822 name in the SAN extension.
		/// </summary>
		public string? Rfc822Name => _SAN.rfc822Names.Count > 0 ? _SAN.rfc822Names.First() : null;

		/// <summary>
		/// Gets the first DNS name in the SAN extension.
		/// </summary>
		public DnsEndPoint? DnsName => _SAN.dnsNames.Count > 0 ? _SAN.dnsNames.First() : null;

		/// <summary>
		/// Gets the first X.400 address in the SAN extension.
		/// </summary>
		public ORAddress? X400Address => _SAN.x400Addresses.Count > 0 ? _SAN.x400Addresses.First() : null;

		/// <summary>
		/// Gets the first directory name in the SAN extension.
		/// </summary>
		public X509Name? DirectoryName => _SAN.directoryNames.Count > 0 ? _SAN.directoryNames.First() : null;

		/// <summary>
		/// Gets the first EDI party name in the SAN extension.
		/// </summary>
		public EdiPartyName? EdiPartyName => _SAN.ediPartyNames.Count > 0 ? _SAN.ediPartyNames.First() : null;

		/// <summary>
		/// Gets the first uniform resource identifier in the SAN extension.
		/// </summary>
		public Uri? UniformResourceIdentifier => _SAN.uniformResourceIdentifiers.Count > 0 ? _SAN.uniformResourceIdentifiers.First() : null;

		/// <summary>
		/// Gets the first IP address in the SAN extension.
		/// </summary>
		public IPAddress? IPAddress => _SAN.iPAddresses.Count > 0 ? _SAN.iPAddresses.First() : null;

		/// <summary>
		/// Gets the first registered ID in the SAN extension.
		/// </summary>
		public DerObjectIdentifier? RegisteredID => _SAN.registeredIDs.Count > 0 ? _SAN.registeredIDs.First() : null;

		/// <summary>
		/// Deconstructs the SAN extension into a FASCN value.
		/// </summary>
		/// <param name="fASCN">The FASCN value.</param>
		public void Deconstruct(
		  out FASCN? fASCN
		) {
			fASCN = FASCN;
		}
		/// <summary>
		/// Deconstructs the SAN extension into a FASCN value and a principal name.
		/// </summary>
		/// <param name="fASCN">The FASCN value.</param>
		/// <param name="principalName">The principal name.</param>
		public void Deconstruct(
		  out FASCN? fASCN,
		  out string? principalName
		) {
			fASCN = FASCN;
			principalName = PrincipalName;
		}
		/// <summary>
		/// Deconstructs the SAN extension into a FASCN value, a principal name, and an RFC 822 name.
		/// </summary>
		/// <param name="fASCN">The FASCN value.</param>
		/// <param name="principalName">The principal name.</param>
		/// <param name="rfc822Name">The RFC 822 name.</param>
		public void Deconstruct(
		  out FASCN? fASCN,
		  out string? principalName,
		  out string? rfc822Name
		) {
			fASCN = FASCN;
			principalName = PrincipalName;
			rfc822Name = Rfc822Name;
		}
		/// <summary>
		/// Deconstructs the SAN extension into a FASCN value, a principal name, an RFC 822 name, a DNS name, an X.400 address, a directory name, an EDI party name, a uniform resource identifier, an IP address, and a registered ID.
		/// </summary>
		/// <param name="fASCN">The FASCN value.</param>
		/// <param name="principalName">The principal name.</param>
		/// <param name="rfc822Name">The RFC 822 name.</param>
		/// <param name="dnsName">The DNS name.</param>
		public void Deconstruct(
		  out FASCN? fASCN,
		  out string? principalName,
		  out string? rfc822Name,
		  out DnsEndPoint? dnsName
		) {
			fASCN = FASCN;
			principalName = PrincipalName;
			rfc822Name = Rfc822Name;
			dnsName = DnsName;
		}
		/// <summary>
		/// Deconstructs the SAN extension into a FASCN value, a principal name, an RFC 822 name, a DNS name, an X.400 address, a directory name, an EDI party name, a uniform resource identifier, an IP address, and a registered ID.
		/// </summary>
		/// <param name="fASCN">The FASCN value.</param>
		/// <param name="principalName">The principal name.</param>
		/// <param name="rfc822Name">The RFC 822 name.</param>
		/// <param name="dnsName">The DNS name.</param>
		/// <param name="x400Address">The X.400 address.</param>
		public void Deconstruct(
		  out FASCN? fASCN,
		  out string? principalName,
		  out string? rfc822Name,
		  out DnsEndPoint? dnsName,
		  out ORAddress? x400Address
		) {
			fASCN = FASCN;
			principalName = PrincipalName;
			rfc822Name = Rfc822Name;
			dnsName = DnsName;
			x400Address = X400Address;
		}
		/// <summary>
		/// Deconstructs the SAN extension into a FASCN value, a principal name, an RFC 822 name, a DNS name, an X.400 address, a directory name, an EDI party name, a uniform resource identifier, an IP address, and a registered ID.
		/// </summary>
		/// <param name="fASCN">The FASCN value.</param>
		/// <param name="principalName">The principal name.</param>
		/// <param name="rfc822Name">The RFC 822 name.</param>
		/// <param name="dnsName">The DNS name.</param>
		/// <param name="x400Address">The X.400 address.</param>
		/// <param name="directoryName">The directory name.</param>
		public void Deconstruct(
		  out FASCN? fASCN,
		  out string? principalName,
		  out string? rfc822Name,
		  out DnsEndPoint? dnsName,
		  out ORAddress? x400Address,
		  out X509Name? directoryName
		) {
			fASCN = FASCN;
			principalName = PrincipalName;
			rfc822Name = Rfc822Name;
			dnsName = DnsName;
			x400Address = X400Address;
			directoryName = DirectoryName;
		}
		/// <summary>
		/// Deconstructs the SAN extension into a FASCN value, a principal name, an RFC 822 name, a DNS name, an X.400 address, a directory name, an EDI party name, a uniform resource identifier, an IP address, and a registered ID.
		/// </summary>
		/// <param name="fASCN">The FASCN value.</param>
		/// <param name="principalName">The principal name.</param>
		/// <param name="rfc822Name">The RFC 822 name.</param>
		/// <param name="dnsName">The DNS name.</param>
		/// <param name="x400Address">The X.400 address.</param>
		/// <param name="directoryName">The directory name.</param>
		/// <param name="ediPartyName">The EDI party name.</param>
		public void Deconstruct(
		  out FASCN? fASCN,
		  out string? principalName,
		  out string? rfc822Name,
		  out DnsEndPoint? dnsName,
		  out ORAddress? x400Address,
		  out X509Name? directoryName,
		  out EdiPartyName? ediPartyName
		) {
			fASCN = FASCN;
			principalName = PrincipalName;
			rfc822Name = Rfc822Name;
			dnsName = DnsName;
			x400Address = X400Address;
			directoryName = DirectoryName;
			ediPartyName = EdiPartyName;
		}
		/// <summary>
		/// Deconstructs the SAN extension into a FASCN value, a principal name, an RFC 822 name, a DNS name, an X.400 address, a directory name, an EDI party name, a uniform resource identifier, an IP address, and a registered ID.
		/// </summary>
		/// <param name="fASCN">The FASCN value.</param>
		/// <param name="principalName">The principal name.</param>
		/// <param name="rfc822Name">The RFC 822 name.</param>
		/// <param name="dnsName">The DNS name.</param>
		/// <param name="x400Address">The X.400 address.</param>
		/// <param name="directoryName">The directory name.</param>
		/// <param name="ediPartyName">The EDI party name.</param>
		/// <param name="uniformResourceIdentifier">The uniform resource identifier.</param>
		public void Deconstruct(
		  out FASCN? fASCN,
		  out string? principalName,
		  out string? rfc822Name,
		  out DnsEndPoint? dnsName,
		  out ORAddress? x400Address,
		  out X509Name? directoryName,
		  out EdiPartyName? ediPartyName,
		  out Uri? uniformResourceIdentifier
		) {
			fASCN = FASCN;
			principalName = PrincipalName;
			rfc822Name = Rfc822Name;
			dnsName = DnsName;
			x400Address = X400Address;
			directoryName = DirectoryName;
			ediPartyName = EdiPartyName;
			uniformResourceIdentifier = UniformResourceIdentifier;
		}
		/// <summary>
		/// Deconstructs the SAN extension into a FASCN value, a principal name, an RFC 822 name, a DNS name, an X.400 address, a directory name, an EDI party name, a uniform resource identifier, an IP address, and a registered ID.
		/// </summary>
		/// <param name="fASCN">The FASCN value.</param>
		/// <param name="principalName">The principal name.</param>
		/// <param name="rfc822Name">The RFC 822 name.</param>
		/// <param name="dnsName">The DNS name.</param>
		/// <param name="x400Address">The X.400 address.</param>
		/// <param name="directoryName">The directory name.</param>
		/// <param name="ediPartyName">The EDI party name.</param>
		/// <param name="uniformResourceIdentifier">The uniform resource identifier.</param>
		/// <param name="iPAddress">The IP address.</param>
		public void Deconstruct(
		  out FASCN? fASCN,
		  out string? principalName,
		  out string? rfc822Name,
		  out DnsEndPoint? dnsName,
		  out ORAddress? x400Address,
		  out X509Name? directoryName,
		  out EdiPartyName? ediPartyName,
		  out Uri? uniformResourceIdentifier,
		  out IPAddress? iPAddress
		) {
			fASCN = FASCN;
			principalName = PrincipalName;
			rfc822Name = Rfc822Name;
			dnsName = DnsName;
			x400Address = X400Address;
			directoryName = DirectoryName;
			ediPartyName = EdiPartyName;
			uniformResourceIdentifier = UniformResourceIdentifier;
			iPAddress = IPAddress;
		}

		/// <summary>
		/// Deconstructs the SAN extension into a FASCN value, a principal name, an RFC 822 name, a DNS name, an X.400 address, a directory name, an EDI party name, a uniform resource identifier, an IP address, and a registered ID.
		/// </summary>
		/// <param name="fASCN">The FASCN value.</param>
		/// <param name="principalName">The principal name.</param>
		/// <param name="rfc822Name">The RFC 822 name.</param>
		/// <param name="dnsName">The DNS name.</param>
		/// <param name="x400Address">The X.400 address.</param>
		/// <param name="directoryName">The directory name.</param>
		/// <param name="ediPartyName">The EDI party name.</param>
		/// <param name="uniformResourceIdentifier">The uniform resource identifier.</param>
		/// <param name="iPAddress">The IP address.</param>
		/// <param name="registeredID">The registered ID.</param>
		public void Deconstruct(
		  out FASCN? fASCN,
		  out string? principalName,
		  out string? rfc822Name,
		  out DnsEndPoint? dnsName,
		  out ORAddress? x400Address,
		  out X509Name? directoryName,
		  out EdiPartyName? ediPartyName,
		  out Uri? uniformResourceIdentifier,
		  out IPAddress? iPAddress,
		  out DerObjectIdentifier? registeredID
		) {
			fASCN = FASCN;
			principalName = PrincipalName;
			rfc822Name = Rfc822Name;
			dnsName = DnsName;
			x400Address = X400Address;
			directoryName = DirectoryName;
			ediPartyName = EdiPartyName;
			uniformResourceIdentifier = UniformResourceIdentifier;
			iPAddress = IPAddress;
			registeredID = RegisteredID;
		}

	}
}
