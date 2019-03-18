using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EmployerFavourites.Api.Security
{
    public static class SecurityServicesCollectionExtensions
    {
        public static void AddADAuthentication(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            if (!hostingEnvironment.IsDevelopment())
            {
                var activeDirectoryConfig = configuration.GetSection("ActiveDirectory").Get<ActiveDirectoryConfiguration>();
                
                services.AddAuthorization(o =>
                {
                    o.AddPolicy("default", policy =>
                    {
                        policy.RequireAuthenticatedUser();
                    });
                });
                services.AddAuthentication(auth =>
                {
                    auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

                }).AddJwtBearer(auth =>
                {
                    auth.Authority = $"https://login.microsoftonline.com/{activeDirectoryConfig.Tenant}";
                    auth.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidAudiences = new List<string>
                        {
                            activeDirectoryConfig.Identifier,
                            activeDirectoryConfig.Id
                        }
                    };
                });
            }
        }
    }
}