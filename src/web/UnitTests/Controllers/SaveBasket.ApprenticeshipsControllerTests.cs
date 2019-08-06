using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sfa.Das.Sas.Shared.Basket.Interfaces;
using Sfa.Das.Sas.Shared.Basket.Models;
using Xunit;
using WriteModel = DfE.EmployerFavourites.Domain.WriteModel;

namespace DfE.EmployerFavourites.Web.UnitTests.Controllers
{
    public partial class ApprenticeshipsControllerTests : ApprenticeshipsControllerTestsBase
    {
        private ApprenticeshipFavouritesBasket _basket;

        public ApprenticeshipsControllerTests()
        {
            _basket = new ApprenticeshipFavouritesBasket();
            _mockBasketStore = new Mock<IApprenticeshipFavouritesBasketStore>();
            _mockBasketStore.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(_basket);
        }

        [Fact]
        public async Task SaveBasket_ReturnsRedirectResult_ForValidBasketId()
        {
            var result = await Sut.SaveBasket(Guid.NewGuid());

            Assert.IsType<RedirectResult>(result);
        }

        [Fact]
        public async Task SaveBasket_ReturnsBadRequest_ForInvalidBasketId()
        {
            var result = await Sut.SaveBasket(Guid.Empty);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task SaveBasket_ReturnRedirectUrlToDashboard_ForAccountSavedAgainst()
        {
            var result = await Sut.SaveBasket(Guid.NewGuid());
            var expectedUrl = string.Format(TEST_MA_ACCOUNT_DASHBOARD_URL, EMPLOYER_ACCOUNT_ID);

            var response = Assert.IsType<RedirectResult>(result);
            Assert.Equal(expectedUrl, response.Url);
        }

        [Fact]
        public async Task SaveBasket_UpdatesFavourites_ForGivenEmployerAccount()
        {
            _basket.Add(new ApprenticeshipFavourite("40"));

            var result = await Sut.SaveBasket(Guid.NewGuid());

            _mockFavouritesWriteRepository.Verify(x =>
                x.SaveApprenticeshipFavourites(EMPLOYER_ACCOUNT_ID, It.IsAny<WriteModel.ApprenticeshipFavourites>()),
                Times.Once);
        }

        [Fact]
        public async Task SaveBasket_AddsSingleApprenticeshipToList_WhenNoFavouritesExist()
        {
            _basket.Add(new ApprenticeshipFavourite("40"));

            var result = await Sut.SaveBasket(Guid.NewGuid());

            _mockFavouritesWriteRepository.Verify(x =>
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(),
                    It.Is<WriteModel.ApprenticeshipFavourites>(a => ContainsExistingPlusNewApprenticeship(a, "40"))),
                Times.Once);
        }

        [Fact]
        public async Task SaveBasket_AddsApprenticeshipAndUkprn_WhenNoFavouritesExist()
        {
            _basket.Add(new ApprenticeshipFavourite("60", 12345678));

            var result = await Sut.SaveBasket(Guid.NewGuid());

            _mockFavouritesWriteRepository.Verify(x =>
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(),
                    It.Is<WriteModel.ApprenticeshipFavourites>(a => ContainsExistingAndNewApprenticeshipWithUkprn(a, "60", 12345678))),
                Times.Once);
        }

        [Fact]
        public async Task SaveBasket_IgnoresApprenticeship_IfItAlreadyExists()
        {
            _basket.Add(new ApprenticeshipFavourite("420-2-1"));

            var result = await Sut.SaveBasket(Guid.NewGuid());

            _mockFavouritesWriteRepository.Verify(x =>
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(),
                    It.IsAny<WriteModel.ApprenticeshipFavourites>()),
                Times.Never);
        }

        [Fact]
        public async Task SaveBasket_IgnoresDuplicateUkprn_IfItAlreadyExistsForApprenticeship()
        {
            _basket.Add(new ApprenticeshipFavourite("70", 12345678));

            var result = await Sut.SaveBasket(Guid.NewGuid());

            _mockFavouritesWriteRepository.Verify(x =>
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(),
                    It.IsAny<WriteModel.ApprenticeshipFavourites>()),
                Times.Never);
        }

        [Fact]
        public async Task SaveBasket_AppendsUkprnToList_WhenTheApprenticeshipAlreadyExistsAndAlreadyHasUkprns()
        {
            _basket.Add(new ApprenticeshipFavourite("30", 12345678));

            var result = await Sut.SaveBasket(Guid.NewGuid());

            _mockFavouritesWriteRepository.Verify(x =>
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(),
                    It.Is<WriteModel.ApprenticeshipFavourites>(a => ContainsNewUkprnForExistingApprenticeship(a, "30", 12345678))),
                Times.Once);
        }

        [Fact]
        public async Task SaveBasket_AddsUkprnToList_WhenTheApprenticeshipAlreadyExistsAndHasNoUkprns()
        {
            _basket.Add(new ApprenticeshipFavourite("30", 12345678));

            var result = await Sut.SaveBasket(Guid.NewGuid());

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
    }
}
