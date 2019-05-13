using System.Linq;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Web.Configuration;
using EmployerFavouritesWeb.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace DfE.EmployerFavourites.Web.Security
{
    public class EmployerAccountHandler : AuthorizationHandler<EmployerAccountRequirement>
    {
        private IOptions<ExternalLinks> _externalLinks;
        public EmployerAccountHandler(IOptions<ExternalLinks> externalLinks)
        {
            _externalLinks = externalLinks;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EmployerAccountRequirement requirement)
        {
            if (context.Resource is AuthorizationFilterContext mvcContext && mvcContext.RouteData.Values.ContainsKey("employerAccountId"))
            {
                if (context.User.HasClaim(c => c.Type.Equals(EmployerClaims.AccountsClaimsTypeIdentifier)))
                {
                    var accountIdFromUrl = mvcContext.RouteData.Values["employerAccountId"].ToString().ToUpper();
                    var employerAccounts = context.User.GetEmployerAccounts().ToList();

                    if (employerAccounts.Any() == false)
                    {
                        mvcContext.Result = new RedirectResult($"{_externalLinks.Value.AccountsRegistrationPage}?returnUrl={mvcContext.HttpContext.Request.GetEncodedUrl()}");

                        //Although not a 'success', the redirect only works if calling Succeed(), please see: https://stackoverflow.com/questions/41707051
                        context.Succeed(requirement);
                    }

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