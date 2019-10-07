using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DfE.EmployerFavourites.Web.UnitTests.Controllers
{
    public partial class ApprenticeshipsControllerTests : ApprenticeshipsControllerTestsBase
    {

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("1")]
        [InlineData("22")]
        [InlineData("AB12")]
        [InlineData("1q3ef")]
        public async Task DeleteTrainingProviderPost_ReturnsBadRequest_ForInvalidEmployerAccountId(string id)
        {
            var result = await Sut.DeleteProviderPost(id,"123",12345,false);

            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("-1")]
        [InlineData("2-1-1")]
        [InlineData("420-222-1")]
        [InlineData("420-2-222")]
        public async Task DeleteTrainingProviderPost_ReturnsBadRequest_ForInvalidApprenticeshipId(string id)
        {
            var result = await Sut.DeleteProviderPost("AB12XY", id, 12345, false);

            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task DeleteTrainingProviderPost_ReturnsBadRequest_ForInvalidUkprn(int ukprn)
        {
            var result = await Sut.DeleteProviderPost("AB12XY", "123", ukprn, false);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteTrainingProviderPost_ReturnsRedirectToActionResult()
        {
            string expectedAction= "TrainingProvider";

            var result = await Sut.DeleteProviderPost(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID,UKPRN, true);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            
            Assert.Equal(expectedAction, redirectResult.ActionName);
            Assert.Equal(APPRENTICESHIPID, redirectResult.RouteValues["apprenticeshipId"]);
            Assert.Equal(EMPLOYER_ACCOUNT_ID, redirectResult.RouteValues["employerAccountId"]);
        }

        [Fact]
        public async Task DeleteTrainingProviderPost_WithFalseConfirmation_ReturnsRedirectToActionResult()
        {
            string expectedAction = "TrainingProvider";

            var result = await Sut.DeleteProviderPost(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID, UKPRN, false);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal(expectedAction, redirectResult.ActionName);
            Assert.Equal(APPRENTICESHIPID, redirectResult.RouteValues["apprenticeshipId"]);
            Assert.Equal(EMPLOYER_ACCOUNT_ID, redirectResult.RouteValues["employerAccountId"]);
        }


    }
}
