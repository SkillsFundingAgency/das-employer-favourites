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
            _basket.Add("40");

            var result = await Sut.SaveBasket(Guid.NewGuid());

            _mockFavouritesWriteRepository.Verify(x =>
                x.SaveApprenticeshipFavourites(EMPLOYER_ACCOUNT_ID, It.IsAny<WriteModel.ApprenticeshipFavourites>()),
                Times.Once);
        }

        [Fact]
        public async Task SaveBasket_AddsSingleApprenticeshipToList_WhenNoFavouritesExist()
        {
            _basket.Add("40");

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
            _basket.Add("60", 12345678);

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
            _basket.Add("420-2-1");

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
            _basket.Add("70", 12345678);

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
            _basket.Add("30", 12345678);

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
            _basket.Add("30", 12345678);

            var result = await Sut.SaveBasket(Guid.NewGuid());

            _mockFavouritesWriteRepository.Verify(x =>
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(),
                    It.Is<WriteModel.ApprenticeshipFavourites>(a => ContainsNewUkprnForExistingApprenticeship(a, "30", 12345678))),
                Times.Once);
        }

        [Fact]
        public async Task SaveBasket_AddsMultipleFavourites()
        {
            var items = new List<Tuple<string, List<int>>>
            {
                { new Tuple<string, List<int>>("66", new List<int> { 12345678 }) },
                { new Tuple<string, List<int>>("67", new List<int> { 98765432 }) },
                { new Tuple<string, List<int>>("68", new List<int> { 98765432 }) }
            };

            foreach(var item in items)
            {
                _basket.Add(item.Item1, item.Item2.First());
            }

            var result = await Sut.SaveBasket(Guid.NewGuid());

            _mockFavouritesWriteRepository.Verify(x =>
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(),
                    It.Is<WriteModel.ApprenticeshipFavourites>(a => ContainsAllNewApprenticeships(a, items))),
                Times.Once);
        }

        [Fact]
        public async Task SaveBasket_DeletesTheBasketAfterItHasSaved()
        {
            var items = new List<Tuple<string, List<int>>>
            {
                { new Tuple<string, List<int>>("66", new List<int> { 12345678 }) },
                { new Tuple<string, List<int>>("67", new List<int> { 98765432 }) },
                { new Tuple<string, List<int>>("68", new List<int> { 98765432 }) }
            };

            foreach (var item in items)
            {
                _basket.Add(item.Item1, item.Item2.First());
            }

            var result = await Sut.SaveBasket(_basket.Id);

            _mockBasketStore.Verify(s => s.RemoveAsync(It.IsAny<Guid>()), Times.Once);
        }

        private static bool ContainsExistingAndNewApprenticeshipWithUkprn(WriteModel.ApprenticeshipFavourites a, string apprenticeshipId, int ukprn)
        {
            return ContainsExistingPlusNewApprenticeship(a, apprenticeshipId)
                && a.Any(b => b.ApprenticeshipId == apprenticeshipId && b.Providers.Select(c => c.Ukprn).Contains(ukprn));
        }

        private static bool ContainsNewUkprnForExistingApprenticeship(WriteModel.ApprenticeshipFavourites a, string apprenticeshipId, int ukprn)
        {
            return a.Count == GetTestRepositoryFavourites().Count
                && a.Any(b => b.ApprenticeshipId == apprenticeshipId && b.Providers.Select(c => c.Ukprn).Contains(ukprn));
        }

        private static bool ContainsExistingPlusNewApprenticeship(WriteModel.ApprenticeshipFavourites a, string apprenticeshipId)
        {
            return a.Count == (GetTestRepositoryFavourites().Count + 1) && a.Any(b => b.ApprenticeshipId == apprenticeshipId);
        }

        private static bool ContainsAllNewApprenticeships(WriteModel.ApprenticeshipFavourites a, List<Tuple<string, List<int>>> newFavourites)
        {
            foreach(var item in newFavourites)
            {
                var matchingItem = a.SingleOrDefault(x => x.ApprenticeshipId == item.Item1);

                if (matchingItem == null || matchingItem.Providers == null || matchingItem.Providers.Count != item.Item2.Count)
                    return false;

                for (int i = 0; i < item.Item2.Count; i++)
                {
                    if (item.Item2[i] != matchingItem.Providers[i].Ukprn)
                        return false;
                }
            }
            return true;
        }
    }
}
