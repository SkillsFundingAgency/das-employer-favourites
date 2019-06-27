using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Api;
using DfE.EmployerFavourites.Api.Domain;
using DfE.EmployerFavourites.Api.IntegrationTests.Stubs;
using DfE.EmployerFavourites.Api.Models;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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

                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appSettings.json", optional: false, reloadOnChange: false);
                });
            });
        }

        [Fact]
        public async Task Get_ApprenticesshipsReturnSuccessAndCorrectContentType()
        {
            // Arrange
            var client = BuildClient();

            // Act
            var response = await client.GetAsync("api/apprenticeships/XYZ123");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Post_Apprenticeship_WithNoExistingRecord()
        {
            var client = BuildClient();

            var response = await client.PutAsync("api/apprenticeships/ABC123",
                new StringContent(JsonConvert.SerializeObject(new List<Favourite> { new Favourite { ApprenticeshipId = "55", Ukprns = new List<int> { 10000030 } } }),
                Encoding.UTF8,
                "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Post_Apprenticeship_WithExistingRecord()
        {
            var client = BuildClient();

            var response = await client.PutAsync("api/apprenticeships/XYZ123",
                new StringContent(JsonConvert.SerializeObject(new List<Favourite> { new Favourite { ApprenticeshipId = "55", Ukprns = new List<int> { 10000030 } } }),
                Encoding.UTF8,
                "application/json"));
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        private HttpClient BuildClient()
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
                   
                });
            })
            .CreateClient();
        }
    }
}
