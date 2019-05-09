using System.Collections.Generic;
using DfE.EmployerFavourites.Api.Security;
using DfE.EmployerFavourites.ApplicationServices.Commands;
using DfE.EmployerFavourites.ApplicationServices.Configuration;
using DfE.EmployerFavourites.ApplicationServices.Domain;
using DfE.EmployerFavourites.ApplicationServices.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Account.Api.Client;

namespace DfE.EmployerFavourites.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddMediatR(typeof(SaveApprenticeshipFavouriteCommand).Assembly);

            services.AddMvc(options => {
                if (!Environment.IsDevelopment())
                {
                    services.AddADAuthentication(Configuration);
                    options.Filters.Add(new AuthorizeFilter("default"));
                }
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton<IAccountApiConfiguration>(x => Configuration.GetSection("AccountApiConfiguration").Get<AccountApiConfiguration>());
            services.AddTransient<IAccountApiClient, AccountApiClient>();
            services.AddTransient<IEmployerAccountRepository, EmployerAccountApiRepository>();

            services.AddScoped<IFavouritesRepository, AzureTableStorageFavouritesRepository>();
            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime, ILogger<Startup> logger)
        {
            applicationLifetime.ApplicationStarted.Register(() => logger.LogInformation("Host fully started"));
            applicationLifetime.ApplicationStopping.Register(() => logger.LogInformation("Host shutting down...waiting to complete requests."));
            applicationLifetime.ApplicationStopped.Register(() => logger.LogInformation("Host fully stopped. All requests processed."));
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
