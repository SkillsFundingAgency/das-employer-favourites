using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Application.Commands;
using DfE.EmployerFavourites.Application.Queries;
using DfE.EmployerFavourites.Domain;
using DfE.EmployerFavourites.Web;
using DfE.EmployerFavourites.Web.Configuration;
using DfE.EmployerFavourites.Web.Controllers;
using DfE.EmployerFavourites.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using Xunit;
using WriteModel = DfE.EmployerFavourites.Domain.WriteModel;
using ReadModel = DfE.EmployerFavourites.Domain.ReadModel;
using DfE.EmployerFavourites.Domain.ReadModel;
using DfE.EmployerFavourites.Web.Application.Exceptions;

namespace DfE.EmployerFavourites.UnitTests.Controllers
{
    public class ApprenticeshipsControllerTests
    {
        public const string TEST_MA_ACCOUNT_DASHBOARD_URL = "https://ma-accounts-home.com/{0}/teams";
        public const string EMPLOYER_ACCOUNT_ID = "XXX123";
        public const string USER_ID = "User123";
        public const string APPRENTICESHIPID = "123";
        public const int UKPRN = 10000020;

        private readonly Mock<IOptions<ExternalLinks>> _mockConfig;
        private Mock<IFavouritesReadRepository> _mockFavouritesReadRepository;
        private Mock<IFavouritesWriteRepository> _mockFavouritesWriteRepository;
        private readonly Mock<IAccountApiClient> _mockAccountApiClient;
        private readonly ApprenticeshipsController _sut;

        public ApprenticeshipsControllerTests()
        {

            _mockFavouritesReadRepository = new Mock<IFavouritesReadRepository>();
            _mockFavouritesReadRepository.Setup(x => x.GetApprenticeshipFavourites(EMPLOYER_ACCOUNT_ID)).ReturnsAsync(GetTestRepositoryFavourites());

            _mockFavouritesWriteRepository = new Mock<IFavouritesWriteRepository>();

            _mockAccountApiClient = new Mock<IAccountApiClient>();
            _mockAccountApiClient.Setup(x => x.GetUserAccounts(USER_ID)).ReturnsAsync(GetListOfAccounts());
            _mockAccountApiClient.Setup(x => x.GetAccount(EMPLOYER_ACCOUNT_ID)).ReturnsAsync(GetAccount());

            _mockConfig = new Mock<IOptions<ExternalLinks>>();
            _mockConfig.Setup(x => x.Value).Returns(new ExternalLinks { AccountsDashboardPage = TEST_MA_ACCOUNT_DASHBOARD_URL });

            ServiceProvider provider = BuildDependencies();
            var mediator = provider.GetService<IMediator>();

            _sut = new ApprenticeshipsController(_mockConfig.Object, mediator, Mock.Of<ILogger<ApprenticeshipsController>>());

            SetupUserInContext();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfApprenticeships()
        {
            var result = await _sut.Index(EMPLOYER_ACCOUNT_ID);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ApprenticeshipFavouritesViewModel>(viewResult.ViewData.Model);
            Assert.Equal(4, model.Items.Count());
        }

        [Fact]
        public async Task Index_ReturnsModel_WithNameOfEmployer()
        {
            var result = await _sut.Index(EMPLOYER_ACCOUNT_ID);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.ViewData.Model as ApprenticeshipFavouritesViewModel;
            
            Assert.Equal("Test Company Ltd", model.EmployerName);
        }

        [Fact]
        public async Task Index_ReturnsModel_WithEmployerAccountId()
        {
            var result = await _sut.Index(EMPLOYER_ACCOUNT_ID);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.ViewData.Model as ApprenticeshipFavouritesViewModel;

            Assert.Equal(EMPLOYER_ACCOUNT_ID, model.EmployerAccountId);
        }

        [Fact]
        public async Task Index_ReturnsModel_ItemsIndicateProgrammeType()
        {
            var result = await _sut.Index(EMPLOYER_ACCOUNT_ID);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.ViewData.Model as ApprenticeshipFavouritesViewModel;

            Assert.True(model.Items.Single(x => x.Id == "420-2-1").IsFramework);
            Assert.False(model.Items.Single(x => x.Id == "30").IsFramework);
        }

        [Fact]
        public async Task Index_ReturnsModel_ItemsContainTitle()
        {
            var result = await _sut.Index(EMPLOYER_ACCOUNT_ID);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.ViewData.Model as ApprenticeshipFavouritesViewModel;

            Assert.Equal("Framework-420-2-1", model.Items.Single(x => x.Id == "420-2-1").Title);
            Assert.Equal("Standard-30", model.Items.Single(x => x.Id == "30").Title);
        }

        [Fact]
        public async Task Index_ReturnsModel_ItemsIndicatesIfItHasProviders()
        {
            var result = await _sut.Index(EMPLOYER_ACCOUNT_ID);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.ViewData.Model as ApprenticeshipFavouritesViewModel;

            Assert.False(model.Items.Single(x => x.Id == "420-2-1").HasTrainingProviders);
            Assert.True(model.Items.Single(x => x.Id == "70").HasTrainingProviders);
        }

        [Fact]
        public async Task Index_ReturnsModel_ItemsContainUkprnOfProvider()
        {
            var result = await _sut.Index(EMPLOYER_ACCOUNT_ID);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.ViewData.Model as ApprenticeshipFavouritesViewModel;

            Assert.Null(model.Items.Single(x => x.Id == "420-2-1").Ukprn);
            Assert.Equal(12345678, model.Items.Single(x => x.Id == "70").Ukprn);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("1")]
        [InlineData("22")]
        [InlineData("AB12")]
        [InlineData("1q3ef")]
        public async Task TrainingProvider_ReturnsBadRequest_ForInvalidEmployerAccountId(string id)
        {
            var result = await _sut.Index(id);

            Assert.IsType<BadRequestResult>(result);
        }
                
        [Fact]
        public async Task TrainingProvider_ReturnsViewResult_WithProviderDetails()
        {
            var result = await _sut.TrainingProvider(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID, UKPRN);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<TrainingProviderViewModel>(viewResult.ViewData.Model);
        }

       [Fact]
       public async Task TrainingProvider_ReturnsModel_WithNameOfTrainingProvider()
       {
           var result = await _sut.TrainingProvider(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID, UKPRN);

           var viewResult = Assert.IsType<ViewResult>(result);
           var model = viewResult.ViewData.Model as TrainingProviderViewModel;

           Assert.Equal("Test Provider Ltd", model.ProviderName);
       }

        // TODO: Uncomment when story delivered to include details.
        //[Fact]
        //public async Task TrainingProvider_ReturnsModel_WithTrainingOptions()
        //{
        //    var result = await _sut.TrainingProvider(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID, UKPRN);

        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    var model = viewResult.ViewData.Model as TrainingProviderViewModel;

        //    Assert.Equal("day release, at your location", model.TrainingOptions);
        //}

        //[Theory]
        //[InlineData(65, "65%")]
        //[InlineData(null, "no data available")]
        //public async Task TrainingProvider_ReturnsModel_WithEmployerSatisfaction(double? value, string expectedValue)
        //{
        //    var result = await _sut.TrainingProvider(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID, UKPRN);

        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    var model = viewResult.ViewData.Model as TrainingProviderViewModel;

        //    Assert.Equal(expectedValue, model.EmployerSatisfaction);
        //}

        //[Theory]
        //[InlineData(65, "65%")]
        //[InlineData(null, "no data available")]
        //public async Task TrainingProvider_ReturnsModel_WithLearnerSatisfaction(double? value, string expectedValue)
        //{
        //    var result = await _sut.TrainingProvider(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID, UKPRN);

        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    var model = viewResult.ViewData.Model as TrainingProviderViewModel;

        //    Assert.Equal(expectedValue, model.LearnerSatisfaction);
        //}

        //[Theory]
        //[InlineData(65, "65%")]
        //[InlineData(null, "no data available")]
        //public async Task TrainingProvider_ReturnsModel_WithAchievementRate(double? value, string expectedValue)
        //{
        //    var result = await _sut.TrainingProvider(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID, UKPRN);

        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    var model = viewResult.ViewData.Model as TrainingProviderViewModel;

        //    Assert.Equal(expectedValue, model.AcheivementRate);
        //}

        [Fact]
        public async Task TrainingProvider_ThrowsException_WhenApprenticeshipNotInFavourites()
        {
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.TrainingProvider(EMPLOYER_ACCOUNT_ID, "66666", UKPRN));
        }

        [Fact]
        public async Task TrainingProvider_ThrowsException_WhenTrainingProviderForApprenticeshipNotInFavourites()
        {
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.TrainingProvider(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID, 99999999));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("-1")]
        [InlineData("2-1-1")]
        [InlineData("420-222-1")]
        [InlineData("420-2-222")]
        public async Task TrainingProvider_ReturnsBadRequest_ForInvalidApprenticeshipId(string id)
        {
            var result = await _sut.TrainingProvider(EMPLOYER_ACCOUNT_ID, id, UKPRN);

            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100000000)]
        [InlineData(9999999)]
        [InlineData(-10000023)]
        public async Task TrainingProvider_ReturnsBadRequest_ForInvalidUkprn(int ukprn)
        {
            var result = await _sut.TrainingProvider(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID, ukprn);

            Assert.IsType<BadRequestResult>(result);
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
        public async Task Add_ReturnRedirectUrlToDashboard_ForAccountSavedAgainst()
        {
            var result = await _sut.Add("30", 12345678);
            var expectedUrl = string.Format(TEST_MA_ACCOUNT_DASHBOARD_URL, EMPLOYER_ACCOUNT_ID);

            var response = Assert.IsType<RedirectResult>(result);
            Assert.Equal(expectedUrl, response.Url);
        }

        [Fact]
        public async Task Add_AddsToList_ForGivenEmployerAccount()
        {
            var result = await _sut.Add("40");

            _mockFavouritesWriteRepository.Verify(x => 
                x.SaveApprenticeshipFavourites(EMPLOYER_ACCOUNT_ID, It.IsAny<WriteModel.ApprenticeshipFavourites>()),
                Times.Once);
        }

        [Fact]
        public async Task Add_AddsApprenticeshipToList_IfDoesntExistAlready()
        {
            var result = await _sut.Add("40");

            _mockFavouritesWriteRepository.Verify(x => 
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(),
                    It.Is<WriteModel.ApprenticeshipFavourites>(a => ContainsExistingPlusNewApprenticeship(a, "40"))),
                Times.Once);
        }

        [Fact]
        public async Task Add_AddsApprenticeshipAndUkprn_WhenTheApprenticeshipDoestExist()
        {
            var result = await _sut.Add("60", 12345678);

            _mockFavouritesWriteRepository.Verify(x =>
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(),
                    It.Is<WriteModel.ApprenticeshipFavourites>(a => ContainsExistingAndNewApprenticeshipWithUkprn(a, "60", 12345678))),
                Times.Once);
        }

        [Fact]
        public async Task Add_IgnoresApprenticeship_IfItAlreadyExists()
        {
             var result = await _sut.Add("420-2-1");

            _mockFavouritesWriteRepository.Verify(x => 
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(),
                    It.IsAny<WriteModel.ApprenticeshipFavourites>()), 
                Times.Never);
        }

        [Fact]
        public async Task Add_IgnoresDuplicateUkprn_IfItAlreadyExistsForApprenticeship()
        {
             var result = await _sut.Add("70", 12345678);

            _mockFavouritesWriteRepository.Verify(x => 
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(), 
                    It.IsAny<WriteModel.ApprenticeshipFavourites>()), 
                Times.Never);
        }

        [Fact]
        public async Task Add_AppendsUkprnToList_WhenTheApprenticeshipAlreadyExistsAndAlreadyHasUkprns()
        {
             var result = await _sut.Add("30", 12345678);

            _mockFavouritesWriteRepository.Verify(x => 
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(), 
                    It.Is<WriteModel.ApprenticeshipFavourites>(a => ContainsNewUkprnForExistingApprenticeship(a, "30", 12345678))), 
                Times.Once);
        }

        [Fact]
        public async Task Add_AddsUkprnToList_WhenTheApprenticeshipAlreadyExistsAndHasNoUkprns()
        {
             var result = await _sut.Add("30", 12345678);

            _mockFavouritesWriteRepository.Verify(x => 
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(), 
                    It.Is<WriteModel.ApprenticeshipFavourites>(a => ContainsNewUkprnForExistingApprenticeship(a, "30", 12345678))), 
                Times.Once);
        }

        private static bool ContainsExistingAndNewApprenticeshipWithUkprn(WriteModel.ApprenticeshipFavourites a, string apprenticeshipId, int ukprn)
        {
            return ContainsExistingPlusNewApprenticeship(a, apprenticeshipId) 
                && a.Any(b => b.ApprenticeshipId == apprenticeshipId && b.Ukprns.Contains(ukprn));
        }

        private static bool ContainsNewUkprnForExistingApprenticeship(WriteModel.ApprenticeshipFavourites a, string apprenticeshipId, int ukprn)
        {
            return a.Count == GetTestRepositoryFavourites().Count 
                && a.Any(b => b.ApprenticeshipId == apprenticeshipId && b.Ukprns.Contains(ukprn));
        }

        private static bool ContainsExistingPlusNewApprenticeship(WriteModel.ApprenticeshipFavourites a, string apprenticeshipId)
        {
            return a.Count == (GetTestRepositoryFavourites().Count + 1) && a.Any(b => b.ApprenticeshipId == apprenticeshipId);
        }

        private static ReadModel.ApprenticeshipFavourites GetTestRepositoryFavourites()
        {
            var list = new ReadModel.ApprenticeshipFavourites();
            list.Add(new ReadModel.ApprenticeshipFavourite("30") { Title = "Standard-30" });
            list.Add(new ReadModel.ApprenticeshipFavourite("420-2-1") { Title = "Framework-420-2-1" });
            list.Add(new ReadModel.ApprenticeshipFavourite("70", new Provider { Ukprn = 12345678 }) { Title = "Standard-70" });
            list.Add(new ReadModel.ApprenticeshipFavourite("123", new Provider { Ukprn = 10000020, Name = "Test Provider Ltd" } ) { Title = "Standard -123" });

            return list;
        }

        private static List<AccountDetailViewModel> GetListOfAccounts()
        {

            return new List<AccountDetailViewModel>
            {
                 new AccountDetailViewModel { AccountId = 7, HashedAccountId = "ABC123" },
                 new AccountDetailViewModel { AccountId = 2, HashedAccountId = "XYZ123" },
                 new AccountDetailViewModel { AccountId = 1, HashedAccountId = "XXX123"},
                 new AccountDetailViewModel { AccountId = 4, HashedAccountId = "AAA123" }
            };
        }


        private AccountDetailViewModel GetAccount()
        {
            return new AccountDetailViewModel
            {
                DasAccountName = "Test Company Ltd"
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
            services.AddTransient<IFavouritesReadRepository>(c => _mockFavouritesReadRepository.Object);
            services.AddTransient<IFavouritesWriteRepository>(c => _mockFavouritesWriteRepository.Object);
            services.AddTransient<IAccountApiClient>(c => _mockAccountApiClient.Object);
            services.AddTransient<ILogger<SaveApprenticeshipFavouriteCommandHandler>>(x => Mock.Of<ILogger<SaveApprenticeshipFavouriteCommandHandler>>());
            services.AddTransient<ILogger<ViewEmployerFavouritesQueryHandler>>(x => Mock.Of<ILogger<ViewEmployerFavouritesQueryHandler>>());
            services.AddTransient<ILogger<ViewTrainingProviderForApprenticeshipFavouriteQueryHandler>>(x => Mock.Of<ILogger<ViewTrainingProviderForApprenticeshipFavouriteQueryHandler>>());
            var provider = services.BuildServiceProvider();
            return provider;
        }
    }
}
