using System;
using System.Collections.Generic;
using System.Security.Claims;
using DfE.EmployerFavourites.Application.Commands;
using DfE.EmployerFavourites.Application.Queries;
using DfE.EmployerFavourites.Domain;
using DfE.EmployerFavourites.Web.Configuration;
using DfE.EmployerFavourites.Web.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using ReadModel = DfE.EmployerFavourites.Domain.ReadModel;
using DfE.EmployerFavourites.Domain.ReadModel;
using Sfa.Das.Sas.Shared.Basket.Interfaces;

namespace DfE.EmployerFavourites.Web.UnitTests.Controllers
{
    public abstract class ApprenticeshipsControllerTestsBase
    {
        protected const string TEST_MA_ACCOUNT_DASHBOARD_URL = "https://ma-accounts-home.com/{0}/teams";
        protected const string TEST_FAT_WEBSITE_APPRENTICESHIP_PAGE_TEMPLATE = "https://fat-website/Apprenticeship/Apprenticeship/{0}/{1}";
        protected const string TEST_FAT_WEBSITE_PROVIDER_PAGE_TEMPLATE = "https://fat-website/Providers/{0}";
        protected const string TEST_CREATE_VACANCY_URL = "https://recruit.at-eas.apprenticeships.education.gov.uk/accounts/{0}/employer-create-vacancy";
        protected const string EMPLOYER_ACCOUNT_ID = "XXX123";
        protected const string USER_ID = "User123";
        protected const string APPRENTICESHIPID = "123";
        protected const string APPRENTICESHIPID_NO_PROVIDER_DATA = "30";
        protected const string APPRENTICESHIPID_WITH_LOCATION = "2";
        protected const int UKPRN = 10000020;
        protected readonly Mock<IOptions<ExternalLinks>> _mockConfig;
        protected readonly Mock<IOptions<FatWebsite>> _mockFatConfig;
        protected Mock<IFavouritesReadRepository> _mockFavouritesReadRepository;
        protected Mock<IFavouritesWriteRepository> _mockFavouritesWriteRepository;
        protected Mock<IApprenticeshipFavouritesBasketStore> _mockBasketStore;
        protected readonly Mock<IAccountApiClient> _mockAccountApiClient;
        protected readonly ApprenticeshipsController Sut;

        protected ApprenticeshipsControllerTestsBase()
        {

            _mockFavouritesReadRepository = new Mock<IFavouritesReadRepository>();
            _mockFavouritesReadRepository.Setup(x => x.GetApprenticeshipFavourites(EMPLOYER_ACCOUNT_ID)).ReturnsAsync(GetTestRepositoryFavourites());

            _mockFavouritesWriteRepository = new Mock<IFavouritesWriteRepository>();
            _mockBasketStore = new Mock<IApprenticeshipFavouritesBasketStore>();

            _mockAccountApiClient = new Mock<IAccountApiClient>();
            _mockAccountApiClient.Setup(x => x.GetUserAccounts(USER_ID)).ReturnsAsync(GetListOfAccounts());
            _mockAccountApiClient.Setup(x => x.GetAccount(EMPLOYER_ACCOUNT_ID)).ReturnsAsync(GetAccount());
            _mockAccountApiClient.Setup(x => x.GetLegalEntitiesConnectedToAccount(EMPLOYER_ACCOUNT_ID)).ReturnsAsync(GetLegalEntitiesConnectedToAccount());

            _mockConfig = new Mock<IOptions<ExternalLinks>>();
            _mockConfig.Setup(x => x.Value).Returns(new ExternalLinks { AccountsDashboardPage = TEST_MA_ACCOUNT_DASHBOARD_URL, CreateVacancyUrl = TEST_CREATE_VACANCY_URL });

            _mockFatConfig = new Mock<IOptions<FatWebsite>>();
            _mockFatConfig.Setup(x => x.Value).Returns(new FatWebsite
            {
                ApprenticeshipPageTemplate = TEST_FAT_WEBSITE_APPRENTICESHIP_PAGE_TEMPLATE,
                ProviderPageTemplate = TEST_FAT_WEBSITE_PROVIDER_PAGE_TEMPLATE
            });

            
            ServiceProvider provider = BuildDependencies();
            var mediator = provider.GetService<IMediator>();

            Sut = new ApprenticeshipsController(_mockConfig.Object, _mockFatConfig.Object, mediator, Mock.Of<ILogger<ApprenticeshipsController>>());

            SetupUserInContext();
        }

        protected static ReadModel.ApprenticeshipFavourites GetTestRepositoryFavourites()
        {
            var list = new ReadModel.ApprenticeshipFavourites();
            list.Add(new ReadModel.ApprenticeshipFavourite("30", new Provider { Ukprn = 10000020, Name = "Test Provider Ltd", Phone = string.Empty, Email = string.Empty, Website = null, EmployerSatisfaction = 0, LearnerSatisfaction = 0 }) { Title = "Standard-30", Level = 4, TypicalLength = 24 });
            list.Add(new ReadModel.ApprenticeshipFavourite("420-2-1") { Title = "Framework-420-2-1", Level = 3, TypicalLength = 18, ExpiryDate = new DateTime(2020, 1, 1) });
            list.Add(new ReadModel.ApprenticeshipFavourite("70", new Provider { Ukprn = 12345678 }) { Title = "Standard-70", Level = 5, TypicalLength = 12 });
            list.Add(new ReadModel.ApprenticeshipFavourite("123", new Provider { Ukprn = 10000020, Name = "Test Provider Ltd", Phone = "020 123 1234", Email = "test@test.com", Website = new Uri("https://www.testprovider.com"), EmployerSatisfaction = 66, LearnerSatisfaction = 99 }) { Title = "Standard-123", Level = 2, TypicalLength = 24 });
            list.Add(new ReadModel.ApprenticeshipFavourite("2", new Provider { Ukprn = 10027893, Name = "Test Provider Ltd", LocationIds = new List<int> { 155399 } }) { Level = 4 });
            //list.Add(new ReadModel.ApprenticeshipFavourite("456", new Provider {
            //    Ukprn = 10000020,
            //    Address = { Primary = "1 Primary Street", Secondary = "Area", ContactType = "LEGAL", Postcode = "PL1 1TP" },
            //    Locations = new List<Location>(),
            //    LocationIds = new List<int>()
            //})); ;
            //list.Add(new ReadModel.ApprenticeshipFavourite("789", new Provider
            //{
            //    Ukprn = 10000020,
            //    Address = { Primary = "1 Primary Street", Secondary = "Area", ContactType = "LEGAL", Postcode = "PL1 1TP" },
            //    Locations = new List<Location> { new Location { Address1 = "Address Line 1", Address2 = "Address Line 2", PostCode = "AA1 2BB", LocationId = 1 } },
            //    LocationIds = new List<int>{ 1 }
            //})); ;
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

        private List<ResourceViewModel> GetLegalEntitiesConnectedToAccount()
        {
            return new List<ResourceViewModel>()
            {
                new ResourceViewModel
                {
                    Id = "123456",
                    Href = "/api/accounts/XXX123/legalentities/123456"
                },
                new ResourceViewModel
                {
                    Id = "123456",
                    Href = "/api/accounts/XXX123/legalentities/123456"
                }
            };
        }

        private void SetupUserInContext()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                        {
                            new Claim("http://das/employer/identity/claims/id", USER_ID)
                        }));

            Sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        private ServiceProvider BuildDependencies()
        {
            var services = new ServiceCollection();
            services.AddMediatR(typeof(Startup).Assembly);
            services.AddTransient<IFavouritesReadRepository>(c => _mockFavouritesReadRepository.Object);
            services.AddTransient<IFavouritesWriteRepository>(c => _mockFavouritesWriteRepository.Object);
            services.AddTransient<IAccountApiClient>(c => _mockAccountApiClient.Object);
            services.AddTransient<IApprenticeshipFavouritesBasketStore>(c => _mockBasketStore.Object);
            services.AddTransient<ILogger<SaveApprenticeshipFavouriteBasketCommandHandler>>(x => Mock.Of<ILogger<SaveApprenticeshipFavouriteBasketCommandHandler>>());
            services.AddTransient<ILogger<ViewEmployerFavouritesQueryHandler>>(x => Mock.Of<ILogger<ViewEmployerFavouritesQueryHandler>>());
            services.AddTransient<ILogger<ViewTrainingProviderForApprenticeshipFavouriteQueryHandler>>(x => Mock.Of<ILogger<ViewTrainingProviderForApprenticeshipFavouriteQueryHandler>>());
            services.AddTransient<ILogger<DeleteApprenticeshipFavouriteCommandHandler>>(x => Mock.Of<ILogger<DeleteApprenticeshipFavouriteCommandHandler>>());
            services.AddTransient<ILogger<DeleteApprenticeshipProviderFavouriteCommandHandler>>(x => Mock.Of<ILogger<DeleteApprenticeshipProviderFavouriteCommandHandler>>());
            services.AddTransient<ILogger<ViewApprenticeshipFavouriteQuery>>(x => Mock.Of<ILogger<ViewApprenticeshipFavouriteQuery>>());

            services.AddTransient<ILogger<EmployerHasLegalEntityQueryHandler>>(x => Mock.Of<ILogger<EmployerHasLegalEntityQueryHandler>>());
            var provider = services.BuildServiceProvider();
            return provider;
        }
    }
}
