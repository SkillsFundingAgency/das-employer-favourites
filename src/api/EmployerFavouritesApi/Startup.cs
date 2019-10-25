using System;
using System.IO;
using System.Reflection;
using DfE.EmployerFavourites.Api.Helpers;
using DfE.EmployerFavourites.Api.Security;
using DfE.EmployerFavourites.Api.Infrastructure.Configuration;
using DfE.EmployerFavourites.Api.Domain;
using DfE.EmployerFavourites.Api.Infrastructure;
using DfE.EmployerFavourites.Api.Infrastructure.Interfaces;
using DfE.EmployerFavourites.Api.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Providers.Api.Client;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using DfE.EmployerFavourites.Api.HealthChecks;
using DfE.EmployerFavourites.Api.Infrastructure.HealthChecks;

namespace DfE.EmployerFavourites.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddADAuthentication(Configuration);
            services.AddMediatR(typeof(GetApprenticeshipFavouritesRequest).Assembly);

            var storageConnectionString = Configuration.GetSection("ConnectionStrings").Get<ConnectionStrings>().AzureStorage;
            
            services.AddHealthChecks()
                .AddAzureTableStorage(storageConnectionString, "table-storage-check")
                .AddCheck<FatApiHealthCheck>("fat-api-check");

            services.AddMvc(options => {
                if (!HostingEnvironment.IsDevelopment())
                {
                    options.Filters.Add(new AuthorizeFilter("default"));
                }
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddTransient<IEmployerAccountRepository, EmployerAccountApiRepository>();

            services.AddTransient<IFatRepository, FatApiRepository>();
            services.AddTransient<IStandardApiClient, StandardApiClient>(service => new StandardApiClient(Configuration.GetValue<string>("FatApiBaseUrl")));
            services.AddTransient<IFrameworkApiClient, FrameworkApiClient>(service => new FrameworkApiClient(Configuration.GetValue<string>("FatApiBaseUrl")));
            services.AddTransient<IProviderApiClient, ProviderApiClient>(service => new ProviderApiClient(Configuration.GetValue<string>("FatApiBaseUrl")));

            services.AddScoped<IFavouritesReadRepository, FatFavouritesTableStorageRepository>();
            services.AddScoped<IFavouritesWriteRepository, FatFavouritesTableStorageRepository>();
            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Employer-Favourites-Api", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime, ILogger<Startup> logger)
        {
            var instance = HostingHelper.GetWebsiteInstanceId();

            applicationLifetime.ApplicationStarted.Register(() => logger.LogInformation($"Host fully started: ({instance})"));
            applicationLifetime.ApplicationStopping.Register(() => logger.LogInformation($"Host shutting down...waiting to complete requests: ({instance})"));
            applicationLifetime.ApplicationStopped.Register(() => logger.LogInformation($"Host fully stopped. All requests processed: ({instance})"));

            if (env.IsDevelopment())
            {
                var configuration = app.ApplicationServices.GetService<Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration>();
                configuration.DisableTelemetry = true;

                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employer Favourites Api V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseMvc();
        }
    }
}
