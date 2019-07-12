using System.IO;
using DfE.EmployerFavourites.Domain;
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
    public abstract class TestBase : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        protected TestBase(WebApplicationFactory<Startup> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.Sources.Clear();

                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appSettings.json", optional: false, reloadOnChange: false);
                    config.AddJsonFile("sharedMenuSettings.json", optional: false, reloadOnChange: false);
                    config.AddJsonFile("linkGeneratorSettings.json", optional: false, reloadOnChange: false);
                });
            });
        }

        protected System.Net.Http.HttpClient BuildClient()
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
