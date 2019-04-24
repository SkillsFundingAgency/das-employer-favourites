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
using Microsoft.Extensions.Logging;

namespace DfE.EmployerFavourites.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _hostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        private IHostingEnvironment _hostingEnvironment;

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

            services.AddMvc(options => {
                // Add media type for csp report.
                var jsonInputFormatters = options.InputFormatters.OfType<JsonInputFormatter>();
                foreach (var formatter in jsonInputFormatters)
                {
                    formatter.SupportedMediaTypes
                        .Add(Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/csp-report"));
                }
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddAuthenticationService(Configuration, _hostingEnvironment);

            services.Configure<ExternalLinks>(Configuration.GetSection("ExternalLinks"));
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Add Content Security Policy
            app.UseCsp(options => options
                .DefaultSources(s => s.Self())
                .StyleSources(s => 
                    {
                        s.Self()
                        .CustomSources("https://www.googletagmanager.com/",
                                        "https://www.tagmanager.google.com/",
                                        "https://tagmanager.google.com/",
                                        "https://fonts.googleapis.com/");
                    }
                )
                .ScriptSources(s =>
                    {
                        s.Self()
                            .CustomSources("https://az416426.vo.msecnd.net",
                                "https://www.google-analytics.com/analytics.js",
                                "https://www.googletagmanager.com/",
                                "https://www.tagmanager.google.com/",
                                "https://tagmanager.google.com/");
                    }
                )
                .FontSources(s => 
                    s.Self()
                    .CustomSources("data:",
                                    "https://fonts.googleapis.com/")
                )
                .ConnectSources(s => 
                    s.Self()
                    .CustomSources("https://dc.services.visualstudio.com")
                )
                .ImageSources(s => 
                    s.Self()
                    .CustomSources("https://maps.googleapis.com", 
                                    "https://www.google-analytics.com", 
                                    "https://ssl.gstatic.com",
                                    "https://www.gstatic.com/",
                                    "data:")
                )
                .ReportUris(r => r.Uris("/ContentPolicyReport/Report")));

            //Registered before static files to always set header
            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opts => opts.NoReferrer());
            app.UseXXssProtection(opts => opts.EnabledWithBlockMode());

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseCookiePolicy();

            //Registered after static files, to set headers for dynamic content.
            app.UseXfo(xfo => xfo.Deny());
            app.UseXDownloadOptions();
            app.UseXRobotsTag(options => options.NoIndex().NoFollow());

            app.UseNoCacheHttpHeaders(); // Effectively forces the browser to always request dynamic pages

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
