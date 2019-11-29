using System.Threading.Tasks;
using DfE.EmployerFavourites.Web.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DfE.EmployerFavourites.Web.IntegrationTests
{
    public class TrainingProviderPageTests : TestBase
    {
        public TrainingProviderPageTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task TrainingProvider_DisplaysProviderDetails_DisplaysCorrectHeadingForSingleFavourite()
        {
            // Arrange
            var client = BuildClient();

            // Act
            var response = await client.GetAsync("accounts/SINGLERESULT/apprenticeships/123/providers");
            var content = await HtmlHelpers.GetDocumentAsync(response);
            var countTextElement = content.QuerySelector(".fav-count-text");

            // Assert
            Assert.Equal("(1) Training Provider", countTextElement.TextContent); // Number of items in the list
        }

        [Fact]
        public async Task TrainingProvider_DisplaysProviderDetails()
        {
            // Arrange
            var client = BuildClient();
            const int ExpectedTrainingProvicerCount = 2;

            // Act
            var response = await client.GetAsync("accounts/ABC123/apprenticeships/123/providers");
            var content = await HtmlHelpers.GetDocumentAsync(response);

            // Assert
            // Counts
            var appTaskListElement = content.QuerySelector(".app-task-list");
            var countTextElement = content.QuerySelector(".fav-count-text");

            // Assert - Counts
            Assert.Equal("(2) Training Providers", countTextElement.TextContent); // Item count in page title
            Assert.Equal(ExpectedTrainingProvicerCount, appTaskListElement.ChildElementCount); // Number of items in the list

            // Provider Properties
            var providerNameElement = content.QuerySelector("a.fav-tp-name");
            var providerPhoneElement = content.QuerySelector(".fav-tp-phone");
            var providerEmailElement = content.QuerySelector(".fav-tp-email>a");
            var providerWebsiteElement = content.QuerySelector(".fav-tp-website>a");
            var providerEmployerSatisfactionElement = content.QuerySelector(".fav-tp-employer-satisfaction");
            var providerLearnerSatisfactionElement = content.QuerySelector(".fav-tp-learner-satisfaction");

            Assert.Equal("Test Provider", providerNameElement.TextContent); // Name
            Assert.Equal("https://findapprenticeshiptraining.apprenticeships.education.gov.uk/Provider/10000020", providerNameElement.Attributes["href"].Value);
            Assert.Equal("020 1234 5678", providerPhoneElement.TextContent); // Phone
            Assert.Equal("test@test.com", providerEmailElement.TextContent); // Email
            Assert.Equal("mailto:test@test.com", providerEmailElement.Attributes["href"].Value); // Website href
            Assert.Equal("https://www.testprovider.com/", providerWebsiteElement.TextContent); // Website
            Assert.Equal("https://www.testprovider.com/", providerWebsiteElement.Attributes["href"].Value); // Website href
            Assert.Equal("86%", providerEmployerSatisfactionElement.TextContent); // Employer Satisfaction
            Assert.Equal("98%", providerLearnerSatisfactionElement.TextContent); // Learner Satisfaction
        }

        [Fact]
        public async Task TrainingProvider_DisplaysHeadOfficeAddressWhenNoLocationsAreSelected()
        {
            // Arrange
            var client = BuildClient();

            // Act
            var response = await client.GetAsync("accounts/NOLOCATIONS/apprenticeships/123/providers");
            var content = await HtmlHelpers.GetDocumentAsync(response);

            // Assert
            var expandContactDetailsElement = content.QuerySelector(".expand-location-details-header");

            Assert.Equal("Head Office - contact details", expandContactDetailsElement.TextContent);

        }

        [Fact]
        public async Task TrainingProvider_DisplaysOneLocationCorrectly()
        {
            // Arrange
            var client = BuildClient();

            // Act
            var response = await client.GetAsync("accounts/ONELOCATION/apprenticeships/123/providers");
            var content = await HtmlHelpers.GetDocumentAsync(response);

            // Assert
            var expandContactDetailsElement = content.QuerySelector(".expand-location-details-header");

            Assert.Equal("1 saved location - view contact details", expandContactDetailsElement.TextContent);

        }
      
    }
}
