using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Infrastructure;
using DfE.EmployerFavourites.Infrastructure.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace DfE.EmployerFavourites.Web.Infrastructure.FavouritesApiClient
{
    public class AdAuthMessageHandler : DelegatingHandler
    {
        private readonly IHostingEnvironment _env;
        private readonly EmployerFavouritesApiConfig _config;

        public AdAuthMessageHandler(IHostingEnvironment env, IOptions<EmployerFavouritesApiConfig> options)
        {
            _env = env;
            _config = options.Value;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var generator = new AdTokenGenerator();
            var token = await generator.Generate(_config.Tenant, _config.ClientId, _config.ClientSecret, _config.IdentifierUri);

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
