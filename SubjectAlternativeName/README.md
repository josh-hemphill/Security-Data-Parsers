# SubjectAlternativeName

[![NuGet](https://img.shields.io/nuget/dt/SecurityDataParsers.SubjectAlternativeName.svg)](https://www.nuget.org/packages/SecurityDataParsers.SubjectAlternativeName) [![NuGet](https://img.shields.io/nuget/vpre/SecurityDataParsers.SubjectAlternativeName.svg)](https://www.nuget.org/packages/SecurityDataParsers.SubjectAlternativeName) [![Release](https://github.com/josh-hemphill/Security-Data-Parsers/actions/workflows/release.yml/badge.svg)](https://github.com/josh-hemphill/Security-Data-Parsers/actions/workflows/release.yml)

A class for digging into all the possible extension data that can be on a certificate.

Some of the more obscure ones are:

  - Federal Agency Smart Credential Number (FASC-N)
  - X400 Address
  - Edi Party Name

See the [IETF Specification](https://datatracker.ietf.org/doc/html/rfc5280#section-4.2.1.6) for more information

<!-- cspell: disable bracketsstartstop -->

## Installation

Install the package with [NuGet](https://www.nuget.org/packages/SecurityDataParsers.SubjectAlternativeName/)

```powershell
    Install-Package SecurityDataParsers.SubjectAlternativeName
```

Or via the .NET Core command line interface:

```shell
    dotnet add package SecurityDataParsers.SubjectAlternativeName
```

Either commands, from Package Manager Console or .NET Core CLI, will download and install the package.

## Usage

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
