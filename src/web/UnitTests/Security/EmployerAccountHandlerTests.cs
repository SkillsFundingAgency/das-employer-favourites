using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
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
        private EmployerAccountHandler _subject;
        private const string TEST_REGISTRATION_URL = "http://test.com";
        private ContextBuilder _builder;

        public EmployerAccountHandlerTests()
        {
            _mockExternalLinks = new Mock<IOptions<ExternalLinks>>();
            _mockExternalLinks.Setup(s => s.Value).Returns(new ExternalLinks
                {
                    AccountsRegistrationPage = TEST_REGISTRATION_URL
                });

            _builder = new ContextBuilder();

            _subject = new EmployerAccountHandler(_mockExternalLinks.Object);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserHasNoAccount_Success() // A strange but necessary result. See https://stackoverflow.com/questions/41707051
        {
            var context =_builder
                .WithAccountIdRouteParameter("ABC123")
                .Build();

            await _subject.HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserHasNoAccounts_RedirectsToRegistration()
        {
            var context = _builder
                .WithAccountIdRouteParameter("ABC123")
                .Build();

            await _subject.HandleAsync(context);

            Assert.IsType<AuthorizationFilterContext>(context.Resource);
            var result = context.Resource as AuthorizationFilterContext;

            Assert.IsType<RedirectResult>(result.Result);
            var redirectResult = result.Result as RedirectResult;
            
            var epectedReturnUrlValue = HttpUtility.UrlEncode($"{ContextBuilder.SCHEME}://{ContextBuilder.HOST}{ContextBuilder.PATH}{ContextBuilder.QUERY_STRING}");
            var expectedRedirecUrl = $"{TEST_REGISTRATION_URL}?returnUrl={epectedReturnUrlValue}";
            Assert.Equal(expectedRedirecUrl, redirectResult.Url);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserHasNoAccounts_RemovesAuthCookie()
        {
            var context = _builder
                .WithAccountIdRouteParameter("ABC123")
                .Build();

            await _subject.HandleAsync(context);

            Assert.IsType<AuthorizationFilterContext>(context.Resource);
            var result = context.Resource as AuthorizationFilterContext;

            Assert.IsType<RedirectResult>(result.Result);
            var redirectResult = result.Result as RedirectResult;

            _builder.AuthServiceMock.Verify(v => v.SignOutAsync(It.IsAny<HttpContext>(), "Cookies", It.IsAny<AuthenticationProperties>()),Times.Once);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserDoesNotHaveMatchingAccountId_Fail()
        {
            var context = _builder
                .WithAccountClaimValue("XYZ123")
                .WithAccountIdRouteParameter("ABC123")
                .Build();

            await _subject.HandleAsync(context);

            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserHasMatchingAccountId_Success()
        {
            var context = _builder
                .WithAccountClaimValue("ABC123")
                .WithAccountIdRouteParameter("ABC123")
                .Build();

            await _subject.HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_RouteDoesNotContainAccountId_Success()
        {
            var context = _builder
                .WithAccountClaimValue("ABC123")
                .Build();

            await _subject.HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }

        internal class ContextBuilder
        {
            public const string SCHEME = "https";
            public const string HOST = "localhost:9200";
            public const string PATH = "/save-apprenticeship";
            public const string QUERY_STRING = "?apprenticeshipId=12&ukprn=12345678";
            public Mock<IAuthenticationService> AuthServiceMock;

            private Claim[] _claims = new Claim[1];
            private Dictionary<string, string> _routeDictionary = new Dictionary<string, string>();

            public ContextBuilder WithAccountClaimValue(string accountId)
            {
                _claims[0] = new Claim(EmployerClaims.AccountsClaimsTypeIdentifier, $"['{accountId}']");
                return this;
            }

            public ContextBuilder WithAccountIdRouteParameter(string accountId)
            {
                _routeDictionary.Add("employerAccountId", accountId);
                return this;
            }

            public AuthorizationHandlerContext Build()
            {
                AuthServiceMock = new Mock<IAuthenticationService>();
                AuthServiceMock
                    .Setup(_ => _.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
                    .Returns(Task.FromResult((object)null));

                var serviceProviderMock = new Mock<IServiceProvider>();
                serviceProviderMock
                    .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                    .Returns(AuthServiceMock.Object);

                var requirements = new[] { new EmployerAccountRequirement() };

                var user = new ClaimsPrincipal(new ClaimsIdentity(_claims, "Basic"));

                var httpContext = new DefaultHttpContext();
                httpContext.Request.Scheme = SCHEME;
                httpContext.Request.Host = new HostString(HOST);
                httpContext.Request.Path = PATH;
                httpContext.Request.QueryString = new QueryString(QUERY_STRING);
                httpContext.RequestServices = serviceProviderMock.Object;
                //httpContext.User = user;

                var routeData = new RouteData(new RouteValueDictionary(_routeDictionary));
                var actionContext = new ActionContext(httpContext, routeData, new ControllerActionDescriptor());
                var resource = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>()) { HttpContext = httpContext };

                return new AuthorizationHandlerContext(requirements, user, resource);
            }
        }
    }
}