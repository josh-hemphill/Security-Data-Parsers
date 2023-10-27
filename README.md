# SecurityDataParsers

[![Codecov](https://img.shields.io/codecov/c/github/josh-hemphill/Security-Data-Parsers.svg?style=?style=plastic)](https://codecov.io/gh/josh-hemphill/Security-Data-Parsers)
[![Test](https://github.com/josh-hemphill/Security-Data-Parsers/actions/workflows/test.yml/badge.svg)](https://github.com/josh-hemphill/Security-Data-Parsers/actions/workflows/test.yml) [![Release](https://github.com/josh-hemphill/Security-Data-Parsers/actions/workflows/release.yml/badge.svg)](https://github.com/josh-hemphill/Security-Data-Parsers/actions/workflows/release.yml) [![GitHub Repo stars](https://img.shields.io/github/stars/josh-hemphill/Security-Data-Parsers?logo=github)](https://github.com/josh-hemphill/Security-Data-Parsers)

A collection small parsing tools to aid in extracting the underlying values in some data formats commonly found in authentication or identity certificates.

<!-- cspell: disable bracketsstartstop -->

Project | Description | NuGet
--- | --- | ---
[FederalAgencySmartCredentialNumber](https://www.nuget.org/packages/SecurityDataParsers.FederalAgencySmartCredentialNumber) | A class for extracting all the good data from FASCN codes (Agency, Org, ID, Employment Type, etc..) | [![NuGet](https://img.shields.io/nuget/vpre/SecurityDataParsers.FederalAgencySmartCredentialNumber.svg)](https://www.nuget.org/packages/SecurityDataParsers.FederalAgencySmartCredentialNumber) [![NuGet](https://img.shields.io/nuget/dt/SecurityDataParsers.FederalAgencySmartCredentialNumber.svg)](https://www.nuget.org/packages/SecurityDataParsers.FederalAgencySmartCredentialNumber)
[SubjectAlternativeName](https://www.nuget.org/packages/SecurityDataParsers.SubjectAlternativeName) | A class for digging into all the possible extension data on certs (e.g. FASCN, X400 Address, Edi Party Name) | [![NuGet](https://img.shields.io/nuget/vpre/SecurityDataParsers.SubjectAlternativeName.svg)](https://www.nuget.org/packages/SecurityDataParsers.SubjectAlternativeName) [![NuGet](https://img.shields.io/nuget/dt/SecurityDataParsers.SubjectAlternativeName.svg)](https://www.nuget.org/packages/SecurityDataParsers.SubjectAlternativeName)

## Installation

Install one of the packages with [NuGet](https://www.nuget.org/)

```powershell
    Install-Package SecurityDataParsers.FederalAgencySmartCredentialNumber
    Install-Package SecurityDataParsers.SubjectAlternativeName
```

Or via the .NET Core command line interface:

```shell
    dotnet add package SecurityDataParsers.FederalAgencySmartCredentialNumber
    dotnet add package SecurityDataParsers.SubjectAlternativeName
```

Either commands, from Package Manager Console or .NET Core CLI, will download and install the packages.

## TODO

  - Better surfacing of the conversions to readable strings for property enums
  - Increasing test coverage

## Usage

### Federal Agency Smart Credential Number (FASCN)

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

### Subject Alternative Name (SAN)

```c#
using SecurityDataParsers.SubjectAlternativeName;

// Load the certificate you want get data from
var cert = new X509Certificate2("path/to/certificate.pfx", "password");

// Get the SAN extension
var sanExtension = cert.Extensions["2.5.29.17"];
// Parse the SAN extension
var san = new SAN(sanExtension);

// Or let the SAN class extract it itself
var san = new SAN(cert);


// Destructure the SAN extension to get its properties
var (
  fASCN,
  principalName,
  rfc822Name,
  dnsName,
  x400Address,
  directoryName,
  ediPartyName,
  uniformResourceIdentifier,
  iPAddress,
  registeredID
) = san.First;

// Use the properties as needed
Console.WriteLine($"FASCN: {fASCN.personIdentifier}");
Console.WriteLine($"Principal Name: {principalName}");

// If it contains multiples, the base class contains lists you can check.
san.dnsNames.Select(v => Console.WrtitLine(v.Host))
```

## API

### See the packages' respective READMEs

  - [FederalAgencySmartCredentialNumber](https://github.com/josh-hemphill/Security-Data-Parsers/tree/latest/FederalAgencySmartCredentialNumber/README.md)
  - [SubjectAlternativeName](https://github.com/josh-hemphill/Security-Data-Parsers/tree/latest/SubjectAlternativeName/README.md)

#### Or the generated API docs

> ### FederalAgencySmartCredentialNumber (FASCN)
>
> See the API docs: [FederalAgencySmartCredentialNumber/docs/README.md](FederalAgencySmartCredentialNumber/docs/README.md)
>
> ### SubjectAlternativeName (SAN)
>
> See the API docs: [SubjectAlternativeName/docs/README.md](SubjectAlternativeName/docs/README.md)

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
