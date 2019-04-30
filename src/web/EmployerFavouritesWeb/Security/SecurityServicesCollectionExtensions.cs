using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Web.Cookies;
using EmployerFavouritesWeb.Security;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.EAS.Account.Api.Client;

namespace DfE.EmployerFavourites.Web.Security
{
    public static class SecurityServicesCollectionExtensions
    {
        public static void AddAuthorizationService(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("EmployerAccountPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(EmployerClaims.AccountsClaimsTypeIdentifier);
                    policy.Requirements.Add(new EmployerAccountRequirement());
                });
            });

            services.AddTransient<IAuthorizationHandler, EmployerAccountHandler>();
        }

        public static void AddAuthenticationService(this IServiceCollection services, OidcConfiguration authConfiguration, IHostingEnvironment hostingEnvironment)
        {
            var authConfig = authConfiguration;

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

                options.Events.OnTokenValidated = async (ctx) =>
                {
                    await PopulateAccountsClaim(ctx, services);
                };

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

        private static async Task PopulateAccountsClaim(TokenValidatedContext ctx, IServiceCollection services)
        {
            var userId = ctx.Principal.GetUserId();
            var sp = services.BuildServiceProvider();
            var accountApiClient = sp.GetService<IAccountApiClient>();
            
            var accounts = await accountApiClient.GetUserAccounts(userId);
            var accountsAsJson = JsonConvert.SerializeObject(accounts.Select(x => x.HashedAccountId));
            var associatedAccountsClaim = new Claim(EmployerClaims.AccountsClaimsTypeIdentifier, accountsAsJson, JsonClaimValueTypes.Json);

            ctx.Principal.Identities.First().AddClaim(associatedAccountsClaim);
        }
    }
}