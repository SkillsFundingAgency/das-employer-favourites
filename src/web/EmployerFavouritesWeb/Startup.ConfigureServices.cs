using System.Linq;
using DfE.EmployerFavourites.ApplicationServices.Commands;
using DfE.EmployerFavourites.ApplicationServices.Configuration;
using DfE.EmployerFavourites.ApplicationServices.Domain;
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
using DfE.EmployerFavourites.ApplicationServices.Infrastructure;
using SFA.DAS.EAS.Account.Api.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DfE.EmployerFavourites.Web
{
    public partial class Startup
    {
        public IConfiguration Configuration { get; }
        private IHostingEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _hostingEnvironment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(SaveApprenticeshipFavouriteCommand).Assembly);

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
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings();
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                return settings;
            };

            services.AddAuthenticationService(Configuration, _hostingEnvironment);

            AddConfiguration(services);
            AddInfrastructureServices(services);
        }

        private void AddInfrastructureServices(IServiceCollection services)
        {
            services.AddSingleton<IAccountApiConfiguration>(x => Configuration.GetSection("AccountApiConfiguration").Get<AccountApiConfiguration>());
            services.AddTransient<IAccountApiClient, AccountApiClient>();

            services.AddScoped<IFavouritesRepository, AzureTableStorageFavouritesRepository>();
        }

        private void AddConfiguration(IServiceCollection services)
        {
            services.Configure<ExternalLinks>(Configuration.GetSection("ExternalLinks"));
            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
        }
    }
}
