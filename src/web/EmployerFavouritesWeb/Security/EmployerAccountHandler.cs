using System.Linq;
using System.Threading.Tasks;
using EmployerFavouritesWeb.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DfE.EmployerFavourites.Web.Security
{
    public class EmployerAccountHandler : AuthorizationHandler<EmployerAccountRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EmployerAccountRequirement requirement)
        {
            if (context.Resource is AuthorizationFilterContext mvcContext && mvcContext.RouteData.Values.ContainsKey("employerAccountId"))
            {
                if (context.User.HasClaim(c => c.Type.Equals(EmployerClaims.AccountsClaimsTypeIdentifier)))
                {
                    var accountIdFromUrl = mvcContext.RouteData.Values["employerAccountId"].ToString().ToUpper();
                    var employerAccounts = context.User.GetEmployerAccounts();

                    if (employerAccounts.Contains(accountIdFromUrl))
                    {
                        context.Succeed(requirement);
                    }
                }
            }
            else
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}