using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Web.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DfE.EmployerFavourites.Web.Security
{
    public static class SecurityServicesCollectionExtensions
    {
        public static void AddAuthenticationService(this IServiceCollection services, OidcConfiguration authConfig, IHostingEnvironment hostingEnvironment)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies", options =>
            {
                options.Cookie.Name = CookieNames.Authentication;

                if (!hostingEnvironment.IsDevelopment())
                {
                    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(OidcConfiguration.SessionTimeoutMinutes);
                }

                options.AccessDeniedPath = "/Error/403";
            })
            .AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = "Cookies";
                options.Authority = authConfig.Authority;
                options.MetadataAddress = authConfig.MetaDataAddress;
                options.RequireHttpsMetadata = false;
                options.ResponseType = "code";
                options.ClientId = authConfig.ClientId;
                options.ClientSecret = authConfig.ClientSecret;
                options.Scope.Add("profile");

                options.Events.OnRemoteFailure = ctx =>
                {
                    if (ctx.Failure.Message.Contains("Correlation failed"))
                    {
                        var logger = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>().CreateLogger<Startup>();
                        logger.LogDebug("Correlation Cookie was invalid - probably timed-out");

                        ctx.Response.Redirect("/");
                        ctx.HandleResponse();
                    }

                    return Task.CompletedTask;
                };
            });
        }
    }
}