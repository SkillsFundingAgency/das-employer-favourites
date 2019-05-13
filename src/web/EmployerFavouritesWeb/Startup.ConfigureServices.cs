using System.Linq;
using DfE.EmployerFavourites.Web.Security;
using DfE.EmployerFavourites.Web.Configuration;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Account.Api.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Authorization;
using SFA.DAS.Employer.Shared.UI;
using DfE.EmployerFavourites.Domain;
using DfE.EmployerFavourites.Infrastructure.Configuration;
using DfE.EmployerFavourites.Infrastructure;
using System;
using System.Collections.Generic;

namespace DfE.EmployerFavourites.Web
{
    public partial class Startup
    {
        public IConfiguration Configuration { get; }
        private IHostingEnvironment _hostingEnvironment;
        private readonly OidcConfiguration _authConfig;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _hostingEnvironment = env;
            _authConfig = Configuration.GetSection("Oidc").Get<OidcConfiguration>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(Startup).Assembly);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc(options =>
            {
                // Add media type for csp report.
                var jsonInputFormatters = options.InputFormatters.OfType<JsonInputFormatter>();
                foreach (var formatter in jsonInputFormatters)
                {
                    formatter.SupportedMediaTypes
                        .Add(Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/csp-report"));
                }

                options.Filters.Add(new AuthorizeFilter("EmployerAccountPolicy"));

            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings();
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                return settings;
            };

            services.AddMaMenuConfiguration(Configuration, RouteNames.Logout_Get, _authConfig.ClientId);

            services.AddAuthenticationService(_authConfig, _hostingEnvironment);
            services.AddAuthorizationService();

            AddConfiguration(services);
            AddInfrastructureServices(services);
        }

        private void AddInfrastructureServices(IServiceCollection services)
        {
            services.AddSingleton<IAccountApiConfiguration>(x => Configuration.GetSection("AccountApiConfiguration").Get<AccountApiConfiguration>());
            services.AddTransient<IAccountApiClient, AccountApiClient>();
            services.AddScoped<AdTokenGenerator>();

            services.AddScoped<ApiFavouritesRepository>();
            services.AddScoped<AzureTableStorageFavouritesRepository>();

            // TODO: This is a temporary measure while we don't have a create method on the API.
            services.AddScoped<Func<string, IFavouritesRepository>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case "Api":
                        return serviceProvider.GetService<ApiFavouritesRepository>();
                    case "Azure":
                        return serviceProvider.GetService<AzureTableStorageFavouritesRepository>();
                    default:
                        throw new KeyNotFoundException(); 
                }
            });
        }

        private void AddConfiguration(IServiceCollection services)
        {
            services.Configure<ExternalLinks>(Configuration.GetSection("ExternalLinks"));
            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<CdnConfig>(Configuration.GetSection("cdn"));
            services.Configure<EmployerFavouritesApiConfig>(Configuration.GetSection("EmployerFavouritesApi"));
        }
    }
}
