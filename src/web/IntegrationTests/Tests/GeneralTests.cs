using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DfE.EmployerFavourites.Web.IntegrationTests
{
    public class GeneralTests : TestBase
    {
        public GeneralTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Theory(Skip = "Broken due to permanent redirect")]
        [InlineData("accounts/ABC123/apprenticeships")]
        [InlineData("accounts/ABC123/apprenticeships/123/providers")]
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
    }
}
