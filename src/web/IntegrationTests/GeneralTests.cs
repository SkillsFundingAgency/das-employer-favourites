using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Domain;
using DfE.EmployerFavourites.IntegrationTests.Helpers;
using DfE.EmployerFavourites.IntegrationTests.Stubs;
using DfE.EmployerFavourites.Web;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Account.Api.Client;
using Xunit;

namespace DfE.EmployerFavourites.IntegrationTests
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

                    var environmentName = hostingContext.HostingEnvironment.EnvironmentName;
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
        public async Task Index_DisplaysListOfFavourites()
        {
            // Arrange
            var client = BuildClient();
            const int ExpectedApprenticeshipCount = 3;

            // Act
            var response = await client.GetAsync("accounts/ABC123/apprenticeships");
            var content = await HtmlHelpers.GetDocumentAsync(response);
            var appNumberElement = content.QuerySelector(".app-hero-container__number");
            var appTaskListElement = content.QuerySelector(".app-task-list");

            // Assert
            Assert.Equal(ExpectedApprenticeshipCount.ToString(), appNumberElement.TextContent); // Test Apprenticeship count
            Assert.Equal(ExpectedApprenticeshipCount, appTaskListElement.ChildElementCount); // Number of items in the list

            var firstAppItemElement = appTaskListElement.Children.First();
            var nameElement = firstAppItemElement.QuerySelector(".app-task-list__heading");
            Assert.Contains("Test Standard1", nameElement.TextContent); // Item contains the name

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
