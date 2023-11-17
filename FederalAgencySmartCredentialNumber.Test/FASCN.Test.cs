
using System.Collections;
using BinaryUtils;

namespace SecurityDataParsers.FederalAgencySmartCredentialNumber.Test;

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
	public class TestDataGenerator : IEnumerable<object[]> {
		private readonly List<object[]> _data = new() {
			new object[] {SampleData.ExampleVariationData.bareFASCN},
			new object[] {SampleData.ExampleVariationData.paddedFASCN},
			new object[] {SampleData.ExampleVariationData.paddedOnlyStartFASCN},
			new object[] {SampleData.ExampleVariationData.paddedOnlyEndFASCN}
		};

		public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
	[Theory]
	[ClassData( typeof( TestDataGenerator ) )]
	public void FASCN_BitOffsets( byte[] src ) {
		FASCN fascn = new( src );

		Assert.Equal( fascn.agencyCode.Digits, SampleData.FieldValues.AgencyCode );
		Assert.Equal( fascn.systemCode.Digits, SampleData.FieldValues.SystemCode );
		Assert.Equal( fascn.credentialNumber.Digits, SampleData.FieldValues.CredentialNumber );
		Assert.Equal( fascn.credentialSeries.Digits, SampleData.FieldValues.CredentialSeries );
		Assert.Equal( fascn.individualCredentialIssue.Digits, SampleData.FieldValues.IndividualCredentialIssue );
		Assert.Equal( fascn.personIdentifier.Digits, SampleData.FieldValues.PersonIdentifier );
		Assert.Equal( fascn.organizationalCategory.Digits, SampleData.FieldValues.OrganizationalCategory );
		Assert.Equal( fascn.organizationIdentifier.Digits, SampleData.FieldValues.OrganizationIdentifier );
		Assert.Equal( fascn.personOrOrganizationAssociationCategory.Digits, SampleData.FieldValues.PersonOrOrganizationAssociationCategory );
	}
}
