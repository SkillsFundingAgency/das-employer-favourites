using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using DfE.EmployerFavourites.Web.Security;
using Newtonsoft.Json;

namespace EmployerFavouritesWeb.Security
{
    public static class ClaimsPrincipalExtensions
     {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(EmployerClaims.IdamsUserIdClaimTypeIdentifier);
        }

        public static IEnumerable<string> GetEmployerAccounts(this ClaimsPrincipal user)
        {
            var employerAccountClaim = user.FindFirst(c => c.Type.Equals(EmployerClaims.AccountsClaimsTypeIdentifier));

            if (string.IsNullOrEmpty(employerAccountClaim?.Value))
                return Enumerable.Empty<string>();
            
            var employerAccounts = JsonConvert.DeserializeObject<List<string>>(employerAccountClaim.Value);
            return employerAccounts;
        }
    }
}