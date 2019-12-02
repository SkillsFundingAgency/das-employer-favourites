using DfE.EmployerFavourites.Infrastructure;
using DfE.EmployerFavourites.Web.Infrastructure.FatApiClient;
using DfE.EmployerFavourites.Web.Infrastructure.FavouritesApiClient;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DfE.EmployerFavourites.Web.UnitTests.Infrastructure
{
    public class FavouritesRepositoryTests
    {
        private readonly Mock<IFavouritesApi> _mockFavouritesApi = new Mock<IFavouritesApi>();
        private readonly Mock<IFatApi> _mockFatApi = new Mock<IFatApi>();
        private readonly FavouritesRepository _sut;

        public FavouritesRepositoryTests()
        {
            _sut = new FavouritesRepository(new NullLogger<FavouritesRepository>(), _mockFavouritesApi.Object, _mockFatApi.Object);
        }

        [Fact]
        public async Task Save_CallsPut_OnFavouritesApi()
        {
            var favourites = new EmployerFavourites.Domain.WriteModel.ApprenticeshipFavourites();

            await _sut.SaveApprenticeshipFavourites("123", favourites);

            _mockFavouritesApi.Verify(x => x.PutAsync("123", favourites));
        }

        [Fact]
        public async Task Save_ShouldRetryIfCallToApiFails()
        {
            var favourites = new EmployerFavourites.Domain.WriteModel.ApprenticeshipFavourites();
            int invocations = 0;
            _mockFavouritesApi.Setup(x => x.PutAsync("123", favourites)).Returns(
                () => 
                {
                    return invocations++ < 1 ? Task.FromException(new Exception()) : Task.CompletedTask; 
                });
               

            await _sut.SaveApprenticeshipFavourites("123", favourites);
        }

        [Fact]
        public async Task DeleteApprenticeshipFavourites_CallsDelete_OnFavouritesApi()
        {
            await _sut.DeleteApprenticeshipFavourites("123", "333");

            _mockFavouritesApi.Verify(x => x.DeleteAsync("123", "333"));
        }

        [Fact]
        public async Task DeleteApprenticeshipFavourites_ShouldRetryIfCallToApiFails()
        {
            int invocations = 0;
            _mockFavouritesApi.Setup(x => x.DeleteAsync("123", "333")).Returns(
                () =>
                {
                    return invocations++ < 1 ? Task.FromException(new Exception()) : Task.CompletedTask;
                });


            await _sut.DeleteApprenticeshipFavourites("123", "333");
        }

        [Fact]
        public async Task DeleteApprenticeshipProviderFavourites_CallsDelete_OnFavouritesApi()
        {
            await _sut.DeleteApprenticeshipProviderFavourites("123", "333", 12345678);

            _mockFavouritesApi.Verify(x => x.DeleteAsync("123", "333", 12345678));
        }

        [Fact]
        public async Task DeleteApprenticeshipProviderFavourites_ShouldRetryIfCallToApiFails()
        {
            int invocations = 0;
            _mockFavouritesApi.Setup(x => x.DeleteAsync("123", "333", 12345678)).Returns(
                () =>
                {
                    return invocations++ < 1 ? Task.FromException(new Exception()) : Task.CompletedTask;
                });


            await _sut.DeleteApprenticeshipProviderFavourites("123", "333", 12345678);
        }

        [Fact]
        public async Task GetApprenticeshipFavourites_CallsGet_OnFavouritesApi()
        {
            await _sut.GetApprenticeshipFavourites("123");

            _mockFavouritesApi.Verify(x => x.GetAsync("123"));
        }

        [Fact]
        public async Task GetApprenticeshipFavourites_Enriches_ApprenticeshipInfo_FromFatApi()
        {
            var favourites = new EmployerFavourites.Domain.ReadModel.ApprenticeshipFavourites
            {
                new DfE.EmployerFavourites.Domain.ReadModel.ApprenticeshipFavourite("123"),
                new DfE.EmployerFavourites.Domain.ReadModel.ApprenticeshipFavourite("435-1-2")
            };

            _mockFavouritesApi.Setup(x => x.GetAsync("123")).ReturnsAsync(favourites);
            _mockFatApi.Setup(x => x.GetStandardAsync("123")).ReturnsAsync(new FatStandard { StandardId = "123", Duration = 2, Level = 4, Title = "Test Standard" });
            _mockFatApi.Setup(x => x.GetFrameworkAsync("435-1-2")).ReturnsAsync(new FatFramework { FrameworkId = "435-1-2", Duration = 1, Level = 3, Title = "Test Framework", ExpiryDate = new DateTime(2020, 9, 30) });

            var result = await _sut.GetApprenticeshipFavourites("123");

            Assert.Equal(2, result.Count);

            var standard = result[0];
            Assert.Equal("123", standard.ApprenticeshipId);
            Assert.Null(standard.ExpiryDate);
            Assert.False(standard.IsFramework);
            Assert.Equal(4, standard.Level);
            // Assert.Equal("Test Standard", standard.Title); // This is not set
            Assert.Equal(2, standard.TypicalLength);

            var framework = result[1];
            Assert.Equal("435-1-2", framework.ApprenticeshipId);
            Assert.Equal(new DateTime(2020, 9, 30), framework.ExpiryDate);
            Assert.True(framework.IsFramework);
            Assert.Equal(3, framework.Level);
            // Assert.Equal("Test Standard", standard.Title); // This is not set
            Assert.Equal(1, framework.TypicalLength);
        }


        [Fact]
        public async Task GetApprenticeshipFavourites_Enriches_ProviderInfo_FromFatApi()
        {
            var favourites = new EmployerFavourites.Domain.ReadModel.ApprenticeshipFavourites
            {
                new DfE.EmployerFavourites.Domain.ReadModel.ApprenticeshipFavourite("123", new EmployerFavourites.Domain.ReadModel.Provider { Ukprn = 12345678 }),
                new DfE.EmployerFavourites.Domain.ReadModel.ApprenticeshipFavourite("435-1-2")
            };

            _mockFavouritesApi.Setup(x => x.GetAsync("123")).ReturnsAsync(favourites);
            _mockFatApi.Setup(x => x.GetStandardAsync("123")).ReturnsAsync(new FatStandard { StandardId = "123", Duration = 2, Level = 4, Title = "Test Standard" });
            _mockFatApi.Setup(x => x.GetFrameworkAsync("435-1-2")).ReturnsAsync(new FatFramework { FrameworkId = "435-1-2", Duration = 1, Level = 3, Title = "Test Framework", ExpiryDate = new DateTime(2020, 9, 30) });

            _mockFatApi.Setup(x => x.GetProviderAsync("12345678")).ReturnsAsync(new FatTrainingProvider { Ukprn = 12345678, Phone = "123123", Email = "email@email.com", Website = new Uri("https://site.com"), EmployerSatisfaction = 123, LearnerSatisfaction = 321,
                Addresses = new System.Collections.Generic.List<ProviderAddress> { new ProviderAddress { ContactType = "PRIMARY", Primary = "1 Training Street", Secondary = "Provider", Town = "Town", Postcode = "AA1 2BB" } } });

            var result = await _sut.GetApprenticeshipFavourites("123");

            var standard = result[0];
            Assert.Equal("123123", standard.Providers[0].Phone);
            Assert.Equal("email@email.com", standard.Providers[0].Email);
            Assert.Equal(new Uri("https://site.com"), standard.Providers[0].Website);
            Assert.Equal(123, standard.Providers[0].EmployerSatisfaction);
            Assert.Equal(321, standard.Providers[0].LearnerSatisfaction);
        }

        [Fact]
        public async Task GetApprenticeshipFavourites_ShouldRetryIfCallToFavouritesApiFails()
        {
            int invocations = 0;
            _mockFavouritesApi.Setup(x => x.GetAsync("123")).ReturnsAsync(
                () =>
                {
                    if (invocations++ < 1) throw new Exception();

                    return new EmployerFavourites.Domain.ReadModel.ApprenticeshipFavourites();
                });


            await _sut.GetApprenticeshipFavourites("123");
        }
    }
}
