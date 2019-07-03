using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using WriteModel = DfE.EmployerFavourites.Domain.WriteModel;

namespace DfE.EmployerFavourites.Web.UnitTests.Controllers
{
    public partial class ApprenticeshipsControllerTests : ApprenticeshipsControllerTestsBase
    {
        [Fact]
        public async Task Add_ReturnsRedirectResult_ForValidStandardId()
        {
            var result = await Sut.Add("20");

            Assert.IsType<RedirectResult>(result);
        }

        [Theory]
        [InlineData("420-12-1")]
        [InlineData("420-1-12")]
        [InlineData("420-13-1")]
        [InlineData("420-13-13")]
        public async Task Add_ReturnsRedirectResult_ForValidFrameworkId(string frameworkId)
        {
            var result = await Sut.Add(frameworkId);

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
            var result = await Sut.Add(id);

            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100000000)]
        [InlineData(9999999)]
        [InlineData(-10000023)]
        public async Task Add_ReturnsBadRequest_ForInvalidUkprn(int ukprn)
        {
            var result = await Sut.Add("30", ukprn);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Add_ReturnRedirectUrlToDashboard_ForAccountSavedAgainst()
        {
            var result = await Sut.Add("30", 12345678);
            var expectedUrl = string.Format(TEST_MA_ACCOUNT_DASHBOARD_URL, EMPLOYER_ACCOUNT_ID);

            var response = Assert.IsType<RedirectResult>(result);
            Assert.Equal(expectedUrl, response.Url);
        }

        [Fact]
        public async Task Add_AddsToList_ForGivenEmployerAccount()
        {
            var result = await Sut.Add("40");

            _mockFavouritesWriteRepository.Verify(x => 
                x.SaveApprenticeshipFavourites(EMPLOYER_ACCOUNT_ID, It.IsAny<WriteModel.ApprenticeshipFavourites>()),
                Times.Once);
        }

        [Fact]
        public async Task Add_AddsApprenticeshipToList_IfDoesntExistAlready()
        {
            var result = await Sut.Add("40");

            _mockFavouritesWriteRepository.Verify(x => 
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(),
                    It.Is<WriteModel.ApprenticeshipFavourites>(a => ContainsExistingPlusNewApprenticeship(a, "40"))),
                Times.Once);
        }

        [Fact]
        public async Task Add_AddsApprenticeshipAndUkprn_WhenTheApprenticeshipDoestExist()
        {
            var result = await Sut.Add("60", 12345678);

            _mockFavouritesWriteRepository.Verify(x =>
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(),
                    It.Is<WriteModel.ApprenticeshipFavourites>(a => ContainsExistingAndNewApprenticeshipWithUkprn(a, "60", 12345678))),
                Times.Once);
        }

        [Fact]
        public async Task Add_IgnoresApprenticeship_IfItAlreadyExists()
        {
             var result = await Sut.Add("420-2-1");

            _mockFavouritesWriteRepository.Verify(x => 
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(),
                    It.IsAny<WriteModel.ApprenticeshipFavourites>()), 
                Times.Never);
        }

        [Fact]
        public async Task Add_IgnoresDuplicateUkprn_IfItAlreadyExistsForApprenticeship()
        {
             var result = await Sut.Add("70", 12345678);

            _mockFavouritesWriteRepository.Verify(x => 
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(), 
                    It.IsAny<WriteModel.ApprenticeshipFavourites>()), 
                Times.Never);
        }

        [Fact]
        public async Task Add_AppendsUkprnToList_WhenTheApprenticeshipAlreadyExistsAndAlreadyHasUkprns()
        {
             var result = await Sut.Add("30", 12345678);

            _mockFavouritesWriteRepository.Verify(x => 
                x.SaveApprenticeshipFavourites(
                    It.IsAny<string>(), 
                    It.Is<WriteModel.ApprenticeshipFavourites>(a => ContainsNewUkprnForExistingApprenticeship(a, "30", 12345678))), 
                Times.Once);
        }

        [Fact]
        public async Task Add_AddsUkprnToList_WhenTheApprenticeshipAlreadyExistsAndHasNoUkprns()
        {
             var result = await Sut.Add("30", 12345678);

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
