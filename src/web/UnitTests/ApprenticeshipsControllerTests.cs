using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Web;
using DfE.EmployerFavourites.Web.Controllers;
using DfE.EmployerFavourites.Web.Configuration;
using DfE.EmployerFavourites.Web.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using SFA.DAS.EAS.Account.Api.Client;
using System.Collections.Generic;
using SFA.DAS.EAS.Account.Api.Types;
using DfE.EmployerFavourites.Web.Commands;

namespace DfE.EmployerFavourites.UnitTests
{
    public class ApprenticeshipsControllerTests
    {
        public const string TEST_MA_ACCOUNTS_HOME_URL = "https://ma-accounts-home.com/";
        public const string EMPLOYER_ACCOUNT_ID = "XXX123";
        public const string USER_ID = "User123";
        private readonly Mock<IOptions<ExternalLinks>> _mockConfig;
        private readonly Mock<IFavouritesRepository> _mockRepository;
        private readonly Mock<IAccountApiClient> _mockAccountApiClient;
        private readonly ApprenticeshipsController _sut;

        public ApprenticeshipsControllerTests()
        {
            _mockRepository = new Mock<IFavouritesRepository>();
            _mockRepository.Setup(x => x.GetApprenticeshipFavourites(EMPLOYER_ACCOUNT_ID)).ReturnsAsync(GetTestRepositoryFavourites());

            _mockAccountApiClient = new Mock<IAccountApiClient>();
            _mockAccountApiClient.Setup(x => x.GetUserAccounts(USER_ID)).ReturnsAsync(GetListOfAccounts());

            _mockConfig = new Mock<IOptions<ExternalLinks>>();
            _mockConfig.Setup(x => x.Value).Returns(new ExternalLinks { AccountsHomePage = new Uri(TEST_MA_ACCOUNTS_HOME_URL) });

            ServiceProvider provider = BuildDependencies();
            var mediator = provider.GetService<IMediator>();

            _sut = new ApprenticeshipsController(Mock.Of<ILogger<ApprenticeshipsController>>(), _mockConfig.Object, mediator);

            SetupUserInContext();
        }

        [Fact]
        public async Task Add_ReturnsRedirectResult_ForValidStandardId()
        {
            var result = await _sut.Add("20");

            Assert.IsType<RedirectResult>(result);
        }

        [Theory]
        [InlineData("420-12-1")]
        [InlineData("420-1-12")]
        [InlineData("420-13-1")]
        [InlineData("420-13-13")]
        public async Task Add_ReturnsRedirectResult_ForValidFrameworkId(string frameworkId)
        {
            var result = await _sut.Add(frameworkId);

            Assert.IsType<RedirectResult>(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("-1")]
        [InlineData("2-1-1")]
        [InlineData("420-222-1")]
        [InlineData("420-2-222")]
        public async Task Add_ReturnsBadRequest_ForInvalidApprenticeshipId(string id)
        {
            var result = await _sut.Add(id);

            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100000000)]
        [InlineData(9999999)]
        [InlineData(-10000023)]
        public async Task Add_ReturnsBadRequest_ForInvalidUkprn(int ukprn)
        {
            var result = await _sut.Add("30", ukprn);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Add_ReturnRedirectUrl_FromConfig()
        {
            var result = await _sut.Add("30", 12345678);

            var response = Assert.IsType<RedirectResult>(result);
            Assert.Equal(TEST_MA_ACCOUNTS_HOME_URL, response.Url);
        }

        [Fact]
        public async Task Add_AddsToList_ForGivenEmployerAccount()
        {
            var result = await _sut.Add("40");

            _mockRepository.Verify(x => 
                x.SaveApprenticeshipFavourites(EMPLOYER_ACCOUNT_ID, It.IsAny<ApprenticeshipFavourites>()),
                Times.Once);
        }

        [Fact]
        public async Task Add_AddsApprenticeshipToList_IfDoesntExistAlready()
        {
            var result = await _sut.Add("40");

            _mockRepository.Verify(x => 
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(),
                    It.Is<ApprenticeshipFavourites>(a => ContainsExistingPlusNewApprenticeship(a, "40"))),
                Times.Once);
        }

        [Fact]
        public async Task Add_AddsApprenticeshipAndUkprn_WhenTheApprenticeshipDoestExist()
        {
            var result = await _sut.Add("60", 12345678);

            _mockRepository.Verify(x =>
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(),
                    It.Is<ApprenticeshipFavourites>(a => ContainsExistingAndNewApprenticeshipWithUkprn(a, "60", 12345678))),
                Times.Once);
        }

        [Fact]
        public async Task Add_IgnoresApprenticeship_IfItAlreadyExists()
        {
             var result = await _sut.Add("50");

            _mockRepository.Verify(x => 
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(),
                    It.IsAny<ApprenticeshipFavourites>()), 
                Times.Never);
        }

                [Fact]
        public async Task Add_IgnoresDuplicateUkprn_IfItAlreadyExistsForApprenticeship()
        {
             var result = await _sut.Add("70", 12345678);

            _mockRepository.Verify(x => 
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(), 
                    It.IsAny<ApprenticeshipFavourites>()), 
                Times.Never);
        }

        [Fact]
        public async Task Add_AppendsUkprnToList_WhenTheApprenticeshipAlreadyExistsAndAlreadyHasUkprns()
        {
             var result = await _sut.Add("30", 12345678);

            _mockRepository.Verify(x => 
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(), 
                    It.Is<ApprenticeshipFavourites>(a => ContainsNewUkprnForExistingApprenticeship(a, "30", 12345678))), 
                Times.Once);
        }

        [Fact]
        public async Task Add_AddsUkprnToList_WhenTheApprenticeshipAlreadyExistsAndHasNoUkprns()
        {
             var result = await _sut.Add("30", 12345678);

            _mockRepository.Verify(x => 
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(), 
                    It.Is<ApprenticeshipFavourites>(a => ContainsNewUkprnForExistingApprenticeship(a, "30", 12345678))), 
                Times.Once);
        }

        private static bool ContainsExistingAndNewApprenticeshipWithUkprn(ApprenticeshipFavourites a, string apprenticeshipId, int ukprn)
        {
            return ContainsExistingPlusNewApprenticeship(a, apprenticeshipId) 
                && a.Any(b => b.ApprenticeshipId == apprenticeshipId && b.Ukprns.Contains(ukprn));
        }

        private static bool ContainsNewUkprnForExistingApprenticeship(ApprenticeshipFavourites a, string apprenticeshipId, int ukprn)
        {
            return a.Count == GetTestRepositoryFavourites().Count 
                && a.Any(b => b.ApprenticeshipId == apprenticeshipId && b.Ukprns.Contains(ukprn));
        }

        private static bool ContainsExistingPlusNewApprenticeship(ApprenticeshipFavourites a, string apprenticeshipId)
        {
            return a.Count == (GetTestRepositoryFavourites().Count + 1) && a.Any(b => b.ApprenticeshipId == apprenticeshipId);
        }

        private static ApprenticeshipFavourites GetTestRepositoryFavourites()
        {
            var list = new ApprenticeshipFavourites();
            list.Add(new ApprenticeshipFavourite("30"));
            list.Add(new ApprenticeshipFavourite("50"));
            list.Add(new ApprenticeshipFavourite("70", 12345678));

            return list;
        }

        private static List<AccountDetailViewModel> GetListOfAccounts()
        {

            return new List<AccountDetailViewModel>
            {
                // Account API in TEST not currently values back correctly. Will rely on the order being deterministic from 
                // the api with the first item being the oldest.
                // new AccountDetailViewModel { HashedAccountId = "ABC123", DateRegistered = new DateTime(2019, 4, 1) },
                // new AccountDetailViewModel { HashedAccountId = "XYZ123", DateRegistered = new DateTime(2019, 3, 1) },
                // new AccountDetailViewModel { HashedAccountId = "XXX123", DateRegistered = new DateTime(2019, 3, 1) },
                // new AccountDetailViewModel { HashedAccountId = "AAA123", DateRegistered = new DateTime(2019, 4, 1) }
                new AccountDetailViewModel { HashedAccountId = "XXX123", DateRegistered = new DateTime(2019, 3, 1) },
                new AccountDetailViewModel { HashedAccountId = "ABC123", DateRegistered = new DateTime(2019, 4, 1) },
                new AccountDetailViewModel { HashedAccountId = "XYZ123", DateRegistered = new DateTime(2019, 3, 1) },
                new AccountDetailViewModel { HashedAccountId = "AAA123", DateRegistered = new DateTime(2019, 4, 1) }
            };
        }

        private void SetupUserInContext()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                        {
                new Claim("http://das/employer/identity/claims/id", USER_ID),
                        }));

            _sut.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        private ServiceProvider BuildDependencies()
        {
            var services = new ServiceCollection();
            services.AddMediatR(typeof(Startup).Assembly);
            services.AddTransient<IFavouritesRepository>(c => _mockRepository.Object);
            services.AddTransient<IAccountApiClient>(c => _mockAccountApiClient.Object);
            services.AddTransient<ILogger<SaveApprenticeshipFavouriteCommandHandler>>(x => Mock.Of<ILogger<SaveApprenticeshipFavouriteCommandHandler>>());
            var provider = services.BuildServiceProvider();
            return provider;
        }
    }
}