﻿using System.Linq;
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
using Microsoft.AspNetCore.Mvc.Authorization;
using SFA.DAS.Employer.Shared.UI;
using DfE.EmployerFavourites.Domain;
using DfE.EmployerFavourites.Infrastructure.Configuration;
using DfE.EmployerFavourites.Infrastructure;
using DfE.EmployerFavourites.Web.Helpers;
using DfE.EmployerFavourites.Web.Infrastructure.FavouritesApiClient;
using System;
using Refit;
using DfE.EmployerFavourites.Web.Infrastructure.FatApiClient;
using SFA.DAS.EmployerUrlHelper.DependencyResolution;
using DfE.EmployerFavourites.Infrastructure.HealthChecks;
using Sfa.Das.Sas.Shared.Basket;

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
            services.AddApplicationInsightsTelemetry();

            services.AddMediatR(typeof(Startup).Assembly);

            services.AddHealthChecks()
                .AddRedis(Configuration.GetValue<string>("BasketConfig:BasketRedisConnectionString"), "basket-redis-check")
                .AddCheck<FavouritesApiHealthCheck>("favourites-api-check")
                .AddCheck<FatApiHealthCheck>("fat-api-check");

            services.AddEmployerUrlHelper(Configuration);

            var basketConfig = Configuration.GetSection("BasketConfig").Get<BasketConfig>();
            services.AddFavouritesBasket(basketConfig.BasketRedisConnectionString, basketConfig.SlidingExpiryDays);

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

            services.AddMaMenuConfiguration(Configuration, RouteNames.Logout_Get, _authConfig.ClientId);

            services.AddAuthenticationService(_authConfig, _hostingEnvironment);
            services.AddAuthorizationService();

            AddConfiguration(services);
            AddInfrastructureServices(services);

            services.AddScoped<ApplicationInsightsJsHelper>();
        }

        private void AddInfrastructureServices(IServiceCollection services)
        {
            services.AddSingleton<IAccountApiConfiguration>(x => Configuration.GetSection("AccountApiConfiguration").Get<AccountApiConfiguration>());
            services.AddTransient<IAccountApiClient, AccountApiClient>();
            services.AddScoped<AdTokenGenerator>();

            services.AddScoped<IFavouritesReadRepository, FavouritesRepository>();
            services.AddScoped<IFavouritesWriteRepository, FavouritesRepository>();

            services.AddTransient<AdAuthMessageHandler>();

            var builder = services.AddRefitClient<IFavouritesApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetValue<string>("EmployerFavouritesApi:ApiBaseUrl")));

            if (!Configuration.GetSection("EmployerFavouritesApi").Get<EmployerFavouritesApiConfig>().HasEmptyProperties())
                builder.AddHttpMessageHandler<AdAuthMessageHandler>();

            services.AddRefitClient<IFatApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetValue<string>("FatApi:ApiBaseUrl")));

        }

        private void AddConfiguration(IServiceCollection services)
        {
            services.Configure<ExternalLinks>(Configuration.GetSection("ExternalLinks"));
            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<CdnConfig>(Configuration.GetSection("cdn"));
            services.Configure<EmployerFavouritesApiConfig>(Configuration.GetSection("EmployerFavouritesApi"));
            services.Configure<FatWebsite>(Configuration.GetSection("FatWebsite"));
            services.Configure<CampaignsWebsite>(Configuration.GetSection("CampaignsWebsite"));
        }
    }
}
