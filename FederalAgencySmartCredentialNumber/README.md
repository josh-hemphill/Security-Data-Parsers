# FederalAgencySmartCredentialNumber

[![NuGet](https://img.shields.io/nuget/dt/SecurityDataParsers.FederalAgencySmartCredentialNumber.svg)](https://www.nuget.org/packages/SecurityDataParsers.FederalAgencySmartCredentialNumber) [![NuGet](https://img.shields.io/nuget/vpre/SecurityDataParsers.FederalAgencySmartCredentialNumber.svg)](https://www.nuget.org/packages/SecurityDataParsers.FederalAgencySmartCredentialNumber) [![Release](https://github.com/josh-hemphill/SecurityDataParsers/actions/workflows/release.yml/badge.svg)](https://github.com/josh-hemphill/SecurityDataParsers/actions/workflows/release.yml)

A class for extracting all the good data from FASCN codes

  - Agency Code: Identifies the government agency issuing the credential
  - System Code: Identifies the system the card is enrolled in and is unique for each site
  - Credential Number: Encoded by the issuing agency. For a given system no duplicate numbers are active.
  - Credential Series: Field is available to reflect major system changes
  - Individual Credential Issue: Usually a 1 will be incremented if a card is replaced due to loss or damaged
  - Person Identifier: Numeric Code used by the identity source to uniquely identify the token carrier
  - Organizational Category: Type of Organization the individual is affiliated with; whether it is Federal, State, Commercial, or Foreign
  - Organization Identifier: The Identifier that identifies the organization the individual is affiliated with.
  - Person Or Organization Association Category: Indicates the affiliation type the individual has with the Organization

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

## Usage/API

>
> ### Under construction 😜
>

## Changelog

Take a look at the [CHANGELOG.md](https://github.com/josh-hemphill/SecurityDataParsers/tree/latest/CHANGELOG.md).

## Contribution

You're free to contribute to this project by submitting [issues](https://github.com/josh-hemphill/SecurityDataParsers/issues) and/or [pull requests](https://github.com/josh-hemphill/SecurityDataParsers/pulls).

Please keep in mind that every change and feature should be covered by
tests.

## License

This project is licensed under [MIT](https://github.com/josh-hemphill/SecurityDataParsers/blob/latest/LICENSE).

## Contributors

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- ALL-CONTRIBUTORS-LIST:END -->
