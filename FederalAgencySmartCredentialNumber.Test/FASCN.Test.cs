
using BinaryUtils;
using SecurityDataParsers.FederalAgencySmartCredentialNumber;

namespace FederalAgencySmartCredentialNumber.Test;

public class FederalAgencySmartCredentialNumberTest {
	private static class SampleData {
		public static readonly string exampleFascnAsBitString = new( """
		11010 00001 00001 11001 01000 10110 00001 00001
		00001 10000 10110 00001 10011 01000 00100 00100
		01101 10110 00001 10110 10000 10110 10000 10000
		10000 01000 01000 01000 11001 11001 11001 11001
		10000 10000 01000 01000 11001 01000 11111 11100
		""".Where( ch => !char.IsWhiteSpace( ch ) ).ToArray() );

		public static class ExampleVariationData {
			public static readonly byte[] bareFASCN = Binary.BitStrToByteArr( exampleFascnAsBitString );
			public static readonly byte[] paddedFASCN = Binary.BitStrToByteArr( $"111111111{exampleFascnAsBitString}11" );
			public static readonly byte[] paddedOnlyStartFASCN = Binary.BitStrToByteArr( $"11111111111{exampleFascnAsBitString}" );
			public static readonly byte[] paddedOnlyEndFASCN = Binary.BitStrToByteArr( $"{exampleFascnAsBitString}11111111111" );
		}

		public static class FieldValues {
			public static readonly byte[] AgencyCode = Binary.IntStrToByteArr( "0032" );
			public static readonly byte[] SystemCode = Binary.IntStrToByteArr( "0001" );
			public static readonly byte[] CredentialNumber = Binary.IntStrToByteArr( "092446" );
			public static readonly byte[] CredentialSeries = Binary.IntStrToByteArr( "0" );
			public static readonly byte[] IndividualCredentialIssue = Binary.IntStrToByteArr( "1" );
			public static readonly byte[] PersonIdentifier = Binary.IntStrToByteArr( "1112223333" );
			public static readonly byte[] OrganizationalCategory = Binary.IntStrToByteArr( "1" );
			public static readonly byte[] OrganizationIdentifier = Binary.IntStrToByteArr( "1223" );
			public static readonly byte[] PersonOrOrganizationAssociationCategory = Binary.IntStrToByteArr( "2" );
		}
	}
	[Fact]
	public void BareFASCN() {
		byte[] bareFASCN = SampleData.ExampleVariationData.bareFASCN;

		FASCN bare = new( bareFASCN );

		Assert.Equal( bare.agencyCode.Digits, SampleData.FieldValues.AgencyCode );
		Assert.Equal( bare.systemCode.Digits, SampleData.FieldValues.SystemCode );
		Assert.Equal( bare.credentialNumber.Digits, SampleData.FieldValues.CredentialNumber );
		Assert.Equal( bare.credentialSeries.Digits, SampleData.FieldValues.CredentialSeries );
		Assert.Equal( bare.individualCredentialIssue.Digits, SampleData.FieldValues.IndividualCredentialIssue );
		Assert.Equal( bare.personIdentifier.Digits, SampleData.FieldValues.PersonIdentifier );
		Assert.Equal( bare.organizationalCategory.Digits, SampleData.FieldValues.OrganizationalCategory );
		Assert.Equal( bare.organizationIdentifier.Digits, SampleData.FieldValues.OrganizationIdentifier );
		Assert.Equal( bare.personOrOrganizationAssociationCategory.Digits, SampleData.FieldValues.PersonOrOrganizationAssociationCategory );
	}
	[Fact]
	public void PaddedFASCN() {
		byte[] paddedFASCN = SampleData.ExampleVariationData.paddedFASCN;
		FASCN padded = new( paddedFASCN );

		Assert.Equal( padded.agencyCode.Digits, SampleData.FieldValues.AgencyCode );
		Assert.Equal( padded.systemCode.Digits, SampleData.FieldValues.SystemCode );
		Assert.Equal( padded.credentialNumber.Digits, SampleData.FieldValues.CredentialNumber );
		Assert.Equal( padded.credentialSeries.Digits, SampleData.FieldValues.CredentialSeries );
		Assert.Equal( padded.individualCredentialIssue.Digits, SampleData.FieldValues.IndividualCredentialIssue );
		Assert.Equal( padded.personIdentifier.Digits, SampleData.FieldValues.PersonIdentifier );
		Assert.Equal( padded.organizationalCategory.Digits, SampleData.FieldValues.OrganizationalCategory );
		Assert.Equal( padded.organizationIdentifier.Digits, SampleData.FieldValues.OrganizationIdentifier );
		Assert.Equal( padded.personOrOrganizationAssociationCategory.Digits, SampleData.FieldValues.PersonOrOrganizationAssociationCategory );
	}
	[Fact]
	public void PaddedOnlyStartFASCN() {
		byte[] paddedOnlyStartFASCN = SampleData.ExampleVariationData.paddedOnlyStartFASCN;
		FASCN paddedOnlyStart = new( paddedOnlyStartFASCN );

		Assert.Equal( paddedOnlyStart.agencyCode.Digits, SampleData.FieldValues.AgencyCode );
		Assert.Equal( paddedOnlyStart.systemCode.Digits, SampleData.FieldValues.SystemCode );
		Assert.Equal( paddedOnlyStart.credentialNumber.Digits, SampleData.FieldValues.CredentialNumber );
		Assert.Equal( paddedOnlyStart.credentialSeries.Digits, SampleData.FieldValues.CredentialSeries );
		Assert.Equal( paddedOnlyStart.individualCredentialIssue.Digits, SampleData.FieldValues.IndividualCredentialIssue );
		Assert.Equal( paddedOnlyStart.personIdentifier.Digits, SampleData.FieldValues.PersonIdentifier );
		Assert.Equal( paddedOnlyStart.organizationalCategory.Digits, SampleData.FieldValues.OrganizationalCategory );
		Assert.Equal( paddedOnlyStart.organizationIdentifier.Digits, SampleData.FieldValues.OrganizationIdentifier );
		Assert.Equal( paddedOnlyStart.personOrOrganizationAssociationCategory.Digits, SampleData.FieldValues.PersonOrOrganizationAssociationCategory );
	}
	[Fact]
	public void PaddedOnlyEndFASCN() {
		byte[] paddedOnlyEndFASCN = SampleData.ExampleVariationData.paddedOnlyEndFASCN;
		FASCN paddedOnlyEnd = new( paddedOnlyEndFASCN );

		Assert.Equal( paddedOnlyEnd.agencyCode.Digits, SampleData.FieldValues.AgencyCode );
		Assert.Equal( paddedOnlyEnd.systemCode.Digits, SampleData.FieldValues.SystemCode );
		Assert.Equal( paddedOnlyEnd.credentialNumber.Digits, SampleData.FieldValues.CredentialNumber );
		Assert.Equal( paddedOnlyEnd.credentialSeries.Digits, SampleData.FieldValues.CredentialSeries );
		Assert.Equal( paddedOnlyEnd.individualCredentialIssue.Digits, SampleData.FieldValues.IndividualCredentialIssue );
		Assert.Equal( paddedOnlyEnd.personIdentifier.Digits, SampleData.FieldValues.PersonIdentifier );
		Assert.Equal( paddedOnlyEnd.organizationalCategory.Digits, SampleData.FieldValues.OrganizationalCategory );
		Assert.Equal( paddedOnlyEnd.organizationIdentifier.Digits, SampleData.FieldValues.OrganizationIdentifier );
		Assert.Equal( paddedOnlyEnd.personOrOrganizationAssociationCategory.Digits, SampleData.FieldValues.PersonOrOrganizationAssociationCategory );
	}
}
