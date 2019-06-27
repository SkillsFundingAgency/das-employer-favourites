using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace DfE.EmployerFavourites.Infrastructure
{
    public class AdTokenGenerator
    {
        public async Task<string> Generate(string tenant, string clientId, string secret, string identifierUri)
        {
            var authority = $"https://login.microsoftonline.com/{tenant}";
            var clientCredential = new ClientCredential(clientId, secret);
            var context = new AuthenticationContext(authority, true);
            var result = await context.AcquireTokenAsync(identifierUri, clientCredential);


            return result.AccessToken;
        }
    }
}