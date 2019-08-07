using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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

        [Fact]
        public async Task Save_RedirectsToAccountsDashboard_OnSuccessfulSave()
        {
            // Arrange
            var client = BuildClient();
            var formValues = new Dictionary<string, string>
            {
                { "basketId", "f848036e39b240329b3ec20b7743392f" }
            };
            
            // Act
            var response = await client.PostAsync("save-apprenticeship-favourites", new FormUrlEncodedContent(formValues));

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("https://test-accounts-dashboard/accounts/ABC123/teams", response.Headers.Location.OriginalString);
        }
    }
}
