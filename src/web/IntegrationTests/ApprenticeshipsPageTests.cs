using System.Linq;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Web.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DfE.EmployerFavourites.Web.IntegrationTests
{
    public class ApprenticeshipsPageTests : TestBase
    {
        public ApprenticeshipsPageTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Index_DisplaysMessageIfHaveNoFavourites()
        {
            // Arrange
            var client = BuildClient();

            // Act
            var response = await client.GetAsync("accounts/NORESULTS/apprenticeships");
            var content = await HtmlHelpers.GetDocumentAsync(response);
            var noFavouritesText = content.QuerySelector(".fav-no-favourites-text");

            // Assert
            Assert.NotNull(noFavouritesText);
        }

        [Fact]
        public async Task Index_DisplaysCorrectHeadingForSingleFavourite()
        {
            // Arrange
            var client = BuildClient();

            // Act
            var response = await client.GetAsync("accounts/SINGLERESULT/apprenticeships");
            var content = await HtmlHelpers.GetDocumentAsync(response);
            var countTextElement = content.QuerySelector(".fav-count-text");

            // Assert
            Assert.Equal("(1) Apprenticeship", countTextElement.TextContent); // Number of items in the list
        }

        [Fact]
        public async Task Index_DisplaysListOfFavourites()
        {
            // Arrange
            var client = BuildClient();
            const int ExpectedApprenticeshipCount = 3;

            // Act
            var response = await client.GetAsync("accounts/ABC123/apprenticeships");
            var content = await HtmlHelpers.GetDocumentAsync(response);
            var appTaskListElement = content.QuerySelector(".app-task-list");
            var countTextElement = content.QuerySelector(".fav-count-text");

            // Assert - Counts
            Assert.Equal("(3) Apprenticeships", countTextElement.TextContent); // Number of items in the list
            Assert.Equal(ExpectedApprenticeshipCount, appTaskListElement.ChildElementCount); // Number of items in the list

            // Assert - Item values
            var firstAppItemElement = appTaskListElement.Children.First();
            var nameElement = firstAppItemElement.QuerySelector(".fav-app-name");
            var levelElement = firstAppItemElement.QuerySelector(".fav-app-level");
            var lengthElement = firstAppItemElement.QuerySelector(".fav-app-length");
            var frameworkWarning = firstAppItemElement.QuerySelector(".fav-framework-expiry");
            Assert.Equal("Test Standard1", nameElement.TextContent); // Item contains the name
            Assert.Contains("3 (equivalent to A levels at grades A to E)", levelElement.TextContent); // Item contains the name
            Assert.Contains("18 months", lengthElement.TextContent); // Item contains the name
            Assert.Null(frameworkWarning); // Should not be showing the warning for a standard

            var frameworkElement = appTaskListElement.Children.Last();
            frameworkWarning = frameworkElement.QuerySelector(".fav-framework-expiry");
            Assert.NotNull(frameworkWarning); // Should show the expiry warning for a framework

            // Assert - Buttons/Links
            // Should only show 'View Training Provider' button if have associated providers
            var secondAppItemElement = appTaskListElement.Children.Skip(1).First();
            var firstProviderButton = firstAppItemElement.QuerySelector("#view-training-providers");
            var secondProviderButton = secondAppItemElement.QuerySelector("#view-training-providers");
            Assert.NotNull(firstProviderButton);
            Assert.Null(secondProviderButton);
            Assert.Equal("/accounts/ABC123/apprenticeships/123/providers/10000020", firstProviderButton.Attributes["href"].Value);
        }
    }
}
