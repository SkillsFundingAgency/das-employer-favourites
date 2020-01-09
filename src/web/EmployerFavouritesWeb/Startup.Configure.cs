using System;
using System.Collections.Generic;
using DfE.EmployerFavourites.Web.Configuration;
using DfE.EmployerFavourites.Web.Helpers;
using DfE.EmployerFavourites.Web.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DfE.EmployerFavourites.Web
{
    public partial class Startup
    {
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env, 
            IApplicationLifetime applicationLifetime,
            IOptions<ExternalLinks> externalLinks,
            IOptions<CdnConfig> cdnConfig, 
            ILogger<Startup> logger)
        {
            var instance = HostingHelper.GetWebsiteInstanceId();
            applicationLifetime.ApplicationStarted.Register(() => logger.LogInformation($"Host fully started: ({instance})"));
            applicationLifetime.ApplicationStopping.Register(() => logger.LogInformation($"Host shutting down...waiting to complete requests: ({instance})"));
            applicationLifetime.ApplicationStopped.Register(() => logger.LogInformation($"Host fully stopped. All requests processed: ({instance})"));

            app.UseStatusCodePagesWithReExecute("/error/{0}");
            
            if (env.IsDevelopment())
            {
                var configuration = app.ApplicationServices.GetService<Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration>();
                configuration.DisableTelemetry = true;

                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/Error");
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
                                        "https://fonts.googleapis.com/",
                                        cdnConfig.Value.Url.AbsoluteUri);
                    }
                )
                .ScriptSources(s =>
                    {
                        s.UnsafeInlineSrc = true;
                        s.Self()
                            .CustomSources("https://az416426.vo.msecnd.net",
                                "https://www.google-analytics.com/analytics.js",
                                "https://www.googletagmanager.com/",
                                "https://www.tagmanager.google.com/",
                                "https://tagmanager.google.com/",
                                "https://ajax.googleapis.com/",
                                cdnConfig.Value.Url.AbsoluteUri);
                    }
                )
                .FontSources(s => 
                    s.Self()
                    .CustomSources("data:",
                                    "https://fonts.googleapis.com/",
                                    cdnConfig.Value.Url.AbsoluteUri)
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
                                    "data:",
                                    cdnConfig.Value.Url.AbsoluteUri)
                )
                .ReportUris(r => r.Uris("/ContentPolicyReport/Report")));

            //Registered before static files to always set header
            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opts => opts.NoReferrer());
            app.UseXXssProtection(opts => opts.EnabledWithBlockMode());

            app.UseAuthentication();

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
            });
            app.UseStaticFiles();
            app.UseCookiePolicy();

            //Registered after static files, to set headers for dynamic content.
            app.UseXfo(xfo => xfo.Deny());
            app.UseXDownloadOptions();
            app.UseXRobotsTag(options => options.NoIndex().NoFollow());
            app.UseRedirectValidation(opts => {
                opts.AllowSameHostRedirectsToHttps();
                opts.AllowedDestinations(GetAllowableDestinations(_authConfig, externalLinks.Value));
            }); //Register this earlier if there's middleware that might redirect.

            app.UseNoCacheHttpHeaders(); // Effectively forces the browser to always request dynamic pages

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static string[] GetAllowableDestinations(OidcConfiguration authConfig, ExternalLinks linksConfig)
        {
            var destinations = new List<string>();

            if (!string.IsNullOrWhiteSpace(authConfig?.Authority))
                destinations.Add(new Uri(authConfig.Authority).GetLeftPart(UriPartial.Authority));
            
            if (!string.IsNullOrWhiteSpace(linksConfig?.AccountsHomePage))
                destinations.Add(new Uri(linksConfig.AccountsHomePage).GetLeftPart(UriPartial.Authority));

            if (!string.IsNullOrWhiteSpace(linksConfig?.AccountsDashboardPage))
                destinations.Add(new Uri(linksConfig.AccountsDashboardPage).GetLeftPart(UriPartial.Authority));

            if (!string.IsNullOrWhiteSpace(linksConfig?.AccountsRegistrationPage))
                destinations.Add(new Uri(linksConfig.AccountsRegistrationPage).GetLeftPart(UriPartial.Authority));

            return destinations.ToArray();
        }
    }
}
