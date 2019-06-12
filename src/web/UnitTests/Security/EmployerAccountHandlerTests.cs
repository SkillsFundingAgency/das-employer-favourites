using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Web.Configuration;
using DfE.EmployerFavourites.Web.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DfE.EmployerFavourites.Web.UnitTests.Security
{
    public class EmployerAccountHandlerTests
    {
        private Mock<IOptions<ExternalLinks>> _mockExternalLinks;
        private string _testRegistrationLink = "http://test.com";

        public EmployerAccountHandlerTests()
        {
            _mockExternalLinks = new Mock<IOptions<ExternalLinks>>();
            _mockExternalLinks.Setup(s => s.Value).Returns(new ExternalLinks()
                {AccountsRegistrationPage = _testRegistrationLink});
        }

        [Fact]
        public async Task HandleRequirementAsync_UserDoesNotHaveMatchingAccountId_Success()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            var requirements = new [] { new EmployerAccountRequirement()};
            var user = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new Claim[] {
                                new Claim(EmployerClaims.AccountsClaimsTypeIdentifier, "")
                            },
                            "Basic")
                        );
                        
            var routeData = new RouteData(new RouteValueDictionary(new Dictionary<string, string> { {"employerAccountId", "ABC123"} }));
            var actionContext = new ActionContext(new DefaultHttpContext(), routeData, new ControllerActionDescriptor());
            var resource = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>()) { HttpContext = new DefaultHttpContext() { RequestServices = serviceProviderMock.Object } };
            var context = new AuthorizationHandlerContext(requirements, user, resource);
            var subject = new EmployerAccountHandler(_mockExternalLinks.Object);

            await subject.HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserDoesNotHaveMatchingAccountId_RedirectsToRegistration()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            var requirements = new[] { new EmployerAccountRequirement() };
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[] {
                        new Claim(EmployerClaims.AccountsClaimsTypeIdentifier, "")
                    },
                    "Basic")
            );

            var routeData = new RouteData(new RouteValueDictionary(new Dictionary<string, string> { { "employerAccountId", "ABC123" } }));
            var actionContext = new ActionContext(new DefaultHttpContext(), routeData, new ControllerActionDescriptor());
            var resource = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>()){HttpContext = new DefaultHttpContext(){RequestServices = serviceProviderMock .Object} };
            var context = new AuthorizationHandlerContext(requirements, user, resource);
            var subject = new EmployerAccountHandler(_mockExternalLinks.Object);

            await subject.HandleAsync(context);

            Assert.IsType<AuthorizationFilterContext>(context.Resource);
            var result = context.Resource as AuthorizationFilterContext;

            Assert.IsType<RedirectResult>(result.Result);
            var redirectResult = result.Result as RedirectResult;

            Assert.Equal(_testRegistrationLink + "?returnUrl=:///",redirectResult.Url);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserDoesNotHaveMatchingAccountId_RemovesAuthCookie()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            var requirements = new[] { new EmployerAccountRequirement() };
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[] {
                        new Claim(EmployerClaims.AccountsClaimsTypeIdentifier, "")
                    },
                    "Basic")
            );

            var routeData = new RouteData(new RouteValueDictionary(new Dictionary<string, string> { { "employerAccountId", "ABC123" } }));
            var actionContext = new ActionContext(new DefaultHttpContext(), routeData, new ControllerActionDescriptor());
            var resource = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>()) { HttpContext = new DefaultHttpContext() { RequestServices = serviceProviderMock.Object} };
            var context = new AuthorizationHandlerContext(requirements, user, resource);
            var subject = new EmployerAccountHandler(_mockExternalLinks.Object);

            await subject.HandleAsync(context);

            Assert.IsType<AuthorizationFilterContext>(context.Resource);
            var result = context.Resource as AuthorizationFilterContext;

            Assert.IsType<RedirectResult>(result.Result);
            var redirectResult = result.Result as RedirectResult;

            Assert.Equal(_testRegistrationLink + "?returnUrl=:///", redirectResult.Url);

            authServiceMock.Verify(v => v.SignOutAsync(It.IsAny<HttpContext>(), "Cookies", It.IsAny<AuthenticationProperties>()),Times.Once);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserDoesNotHaveMatchingAccountId_Fail()
        {
            var requirements = new[] { new EmployerAccountRequirement() };
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[] {
                        new Claim(EmployerClaims.AccountsClaimsTypeIdentifier, "['XYZ123']")
                    },
                    "Basic")
            );

            var routeData = new RouteData(new RouteValueDictionary(new Dictionary<string, string> { { "employerAccountId", "ABC123" } }));
            var actionContext = new ActionContext(new DefaultHttpContext(), routeData, new ControllerActionDescriptor());
            var resource = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            var context = new AuthorizationHandlerContext(requirements, user, resource);
            var subject = new EmployerAccountHandler(_mockExternalLinks.Object);

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
            var subject = new EmployerAccountHandler(_mockExternalLinks.Object);

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
            var subject = new EmployerAccountHandler(_mockExternalLinks.Object);

          

            await subject.HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }
    }
}