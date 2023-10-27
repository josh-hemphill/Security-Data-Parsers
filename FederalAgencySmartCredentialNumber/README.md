# FederalAgencySmartCredentialNumber

[![NuGet](https://img.shields.io/nuget/dt/SecurityDataParsers.FederalAgencySmartCredentialNumber.svg)](https://www.nuget.org/packages/SecurityDataParsers.FederalAgencySmartCredentialNumber) [![NuGet](https://img.shields.io/nuget/vpre/SecurityDataParsers.FederalAgencySmartCredentialNumber.svg)](https://www.nuget.org/packages/SecurityDataParsers.FederalAgencySmartCredentialNumber) [![Release](https://github.com/josh-hemphill/Security-Data-Parsers/actions/workflows/release.yml/badge.svg)](https://github.com/josh-hemphill/Security-Data-Parsers/actions/workflows/release.yml)

A class for extracting all the good data from FASCN codes

  - Agency Code: Identifies the government agency issuing the credential
  - System Code: Identifies the system the card is enrolled in and is unique for each site
  - Credential Number: Encoded by the issuing agency. For a given system no duplicate numbers are active.
  - Credential Series: Field is available to reflect major system changes
  - Individual Credential Issue: Usually a 1 will be incremented if a card is replaced due to loss or damaged
  - Person Identifier: Numeric Code used by the identity source to uniquely identify the token carrier
  - Organizational Category: Type of Organization the individual is affiliated with; whether it is Federal, State, Commercial, or Foreign
  - Organization Identifier: The Identifier that identifies the organization the individual is affiliated with.
  - Person Or Organization Association Category: Indicates the affiliation type the individual has with the Organization, including their employment type.

For more info see the [CAC Implementation Guide](https://www.cac.mil/Portals/53/Documents/CAC_NG_Implementation_Guide_v2.6.pdf) or the [CAC Endpoint Implementation Guide](https://www.cac.mil/Portals/53/Documents/ref1.c.i-CAC_End-Point_Implementation_Guide_v1-22.pdf)

Or the [NIST specification](https://csrc.nist.gov/pubs/sp/800/73/4/upd1/final)

<!-- cspell: disable bracketsstartstop -->

## Installation

Install the package with [NuGet](https://www.nuget.org/packages/SecurityDataParsers.FederalAgencySmartCredentialNumber/)

```powershell
    Install-Package SecurityDataParsers.FederalAgencySmartCredentialNumber
```

Or via the .NET Core command line interface:

```shell
    dotnet add package SecurityDataParsers.FederalAgencySmartCredentialNumber
```

Either commands, from Package Manager Console or .NET Core CLI, will download and install the package.

## Usage

```C#

  using SecurityDataParsers.FederalAgencySmartCredentialNumber;
  // Load the smart card certificate, or any cert you want to check for a FASCN
  X509Certificate2 cert = new X509Certificate2("path/to/certificate.pfx", "password");
  // Smart card certificates are usually cached in windows personal cert store,
  // so you can pull it as only the public portion and still pull the FASCN off it.

  // Create a new FASCN object
  FASCN fascnObj = FASCN.fromCertificate(cert);

  // Extract the identifying properties from the FASCN object
  
  // Identifies the government agency issuing the credential
  string agencyCode = fascnObj.AgencyCode.GetFriendlyName();
  
  // Identifies the system the card is enrolled in and is unique for each site
  // 4 digits stored as bytes
  (byte, byte, byte, byte) systemCode = fascnObj.SystemCode.AsTuple();
  
  // Encoded by the issuing agency. For a given system no duplicate numbers are active.
  // 6 digits stored as bytes, too many possibilities to enumerate all possible friendly names, so it's up to the user to find agency codes.
  (byte, byte, byte, byte, byte, byte) credentialNumber = fascnObj.CredentialNumber.AsTuple();
  
  // Single reserved digit. Field is available to reflect major system changes
  ValueTuple<byte> credentialSeries = fascnObj.CredentialSeries.AsTuple();
  
  // Usually a 1, but will be incremented if a card is replaced due to loss or damaged
  ValueTuple<byte> individualCredentialIssue = fascnObj.IndividualCredentialIssue.AsTuple();
  
  // Numeric Code used by the identity source to uniquely identify the token carrier
  // 10 digits stored as bytes, this is what is usually the User ID
  byte[] personIdentifier = fascnObj.PersonIdentifier.Digits;
  
  // Type of Organization the individual is affiliated with; whether it is Federal, State, Commercial, or Foreign
  string organizationalCategory = fascnObj.OrganizationalCategory.GetFriendlyName();
  
  // The Identifier that identifies the organization the individual is affiliated with.
  // 4 digits stored as bytes
  (byte, byte, byte, byte) organizationIdentifier = fascnObj.OrganizationIdentifier.AsTuple();
  
  // Indicates the affiliation type the individual has with the Organization, including their employment type.
  string personOrOrganizationAssociationCategory = fascnObj.PersonOrOrganizationAssociationCategory.GetFriendlyName();

```

## API

> See the API docs: [docs/README.md](docs/README.md)

## Changelog

Take a look at the [CHANGELOG.md](https://github.com/josh-hemphill/Security-Data-Parsers/tree/latest/CHANGELOG.md).

## Contribution

You're free to contribute to this project by submitting [issues](https://github.com/josh-hemphill/Security-Data-Parsers/issues) and/or [pull requests](https://github.com/josh-hemphill/Security-Data-Parsers/pulls).

Please keep in mind that every change and feature should be covered by
tests.

## License

This project is licensed under [MIT](https://github.com/josh-hemphill/Security-Data-Parsers/blob/latest/LICENSE).

## Contributors

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- ALL-CONTRIBUTORS-LIST:END -->
