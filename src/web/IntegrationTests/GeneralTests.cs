using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Domain;
using DfE.EmployerFavourites.Web.IntegrationTests.Helpers;
using DfE.EmployerFavourites.Web.IntegrationTests.Stubs;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Account.Api.Client;
using Xunit;

namespace DfE.EmployerFavourites.Web.IntegrationTests
{
    public class GeneralTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public GeneralTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.Sources.Clear();

                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appSettings.json", optional: false, reloadOnChange: false);
                    config.AddJsonFile("sharedMenuSettings.json", optional: false, reloadOnChange: false);
                });
            });
        }

        [Theory]
        [InlineData("accounts/ABC123/apprenticeships")]
        [InlineData("accounts/ABC123/apprenticeships/123/providers/10000020")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = BuildClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
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

        [Fact]
        public async Task TrainingProvider_DisplaysProviderDetails()
        {
            // Arrange
            var client = BuildClient();

            // Act
            var response = await client.GetAsync("accounts/ABC123/apprenticeships/123/providers/10000020");
            var content = await HtmlHelpers.GetDocumentAsync(response);
            var providerNameElement = content.QuerySelector(".app-task-list__heading");

            Assert.Equal("Test Provider", providerNameElement.TextContent);
        }

        [Fact]
        public async Task Save_RedirectsToAccountsDashboard_OnSuccessfulSave()
        {
            // Arrange
            var client = BuildClient();

            // Act
            var response = await client.GetAsync("save-apprenticeship-favourites?apprenticeshipId=345");

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("https://test-accounts-dashboard/accounts/ABC123/teams", response.Headers.Location.OriginalString);
        }

        private System.Net.Http.HttpClient BuildClient()
        {
            return _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddMvc(options =>
                    {
                        options.Filters.Add(new AllowAnonymousFilter());
                    });
                    services.AddScoped<IFavouritesReadRepository, StubFavouritesRepository>();
                    services.AddScoped<IFavouritesWriteRepository, StubFavouritesRepository>();
                    services.AddScoped<IAccountApiClient, StubAccountApiClient>();
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }
    }
}
