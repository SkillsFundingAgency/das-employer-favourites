using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DfE.EmployerFavourites.Web.IntegrationTests
{
    public class SaveFavouritesEndpointTests : TestBase
    {
        public SaveFavouritesEndpointTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact(Skip = "Broken due to permanent redirect")]
        public async Task Save_RedirectsToAccountsDashboard_OnSuccessfulSave()
        {
            // Arrange
            var client = BuildClient();
            
            // Act
            var response = await client.GetAsync("save-apprenticeship-favourites?basketId=f848036e39b240329b3ec20b7743392f");

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("https://test-accounts-dashboard/accounts/ABC123/teams", response.Headers.Location.OriginalString);
        }
    }
}
