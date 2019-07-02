using System.Threading.Tasks;
using DfE.EmployerFavourites.Web.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DfE.EmployerFavourites.Web.IntegrationTests
{
    public class TraingProviderPageTests : TestBase
    {
        public TraingProviderPageTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
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
    }
}
