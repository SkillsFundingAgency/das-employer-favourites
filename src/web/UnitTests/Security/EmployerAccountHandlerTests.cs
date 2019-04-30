using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Xunit;

namespace DfE.EmployerFavourites.UnitTests.Security
{
    public class EmployerAccountHandlerTests
    {

        public EmployerAccountHandlerTests()
        {

        }

        [Fact]
        public async Task HandleRequirementAsync_UserDoesNotHaveMatchingAccountId_Fails()
        {
            var requirements = new [] { new EmployerAccountRequirement()};
            var user = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new Claim[] {
                                new Claim(EmployerClaims.AccountsClaimsTypeIdentifier, "['XYZ123']")
                            },
                            "Basic")
                        );
                        
            var routeData = new RouteData(new RouteValueDictionary(new Dictionary<string, string> { {"employerAccountId", "ABC123"} }));
            var actionContext = new ActionContext(new DefaultHttpContext(), routeData, new ControllerActionDescriptor());
            var resource = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            var context = new AuthorizationHandlerContext(requirements, user, resource);
            var subject = new EmployerAccountHandler();

            await subject.HandleAsync(context);

            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserHasMatchingAccountId_Success()
        {
            var requirements = new [] { new EmployerAccountRequirement()};
            var user = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new Claim[] {
                                new Claim(EmployerClaims.AccountsClaimsTypeIdentifier, "['ABC123']")
                            },
                            "Basic")
                        );
                        
            var routeData = new RouteData(new RouteValueDictionary(new Dictionary<string, string> { {"employerAccountId", "ABC123"} }));
            var actionContext = new ActionContext(new DefaultHttpContext(), routeData, new ControllerActionDescriptor());
            var resource = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            var context = new AuthorizationHandlerContext(requirements, user, resource);
            var subject = new EmployerAccountHandler();

            await subject.HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_RouteDoesNotContainAccountId_Success()
        {
            var requirements = new [] { new EmployerAccountRequirement()};
            var user = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new Claim[] {
                                new Claim(EmployerClaims.AccountsClaimsTypeIdentifier, "['ABC123']")
                            },
                            "Basic")
                        );
                        
            var routeData = new RouteData(new RouteValueDictionary(new Dictionary<string, string>())); // No matching route parameter
            var actionContext = new ActionContext(new DefaultHttpContext(), routeData, new ControllerActionDescriptor());
            var resource = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            var context = new AuthorizationHandlerContext(requirements, user, resource);
            var subject = new EmployerAccountHandler();

            await subject.HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }
    }
}