using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
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
        public async Task DeletePost_ReturnsBadRequest_ForInvalidEmployerAccountId(string id)
        {
            var result = await Sut.DeletePost(id,APPRENTICESHIPID,false);

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
        public async Task DeletePost_ReturnsBadRequest_ForInvalidApprenticeshipId(string id)
        {
            var result = await Sut.DeletePost(EMPLOYER_ACCOUNT_ID, id, false);

            Assert.IsType<BadRequestResult>(result);
        }

       
        [Fact]
        public async Task DeletePost_ReturnsRedirectToActionResult()
        {
            string expectedAction= "Index";

            var result = await Sut.DeletePost(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID, true);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            
            Assert.Equal(expectedAction, redirectResult.ActionName);
            Assert.Equal(EMPLOYER_ACCOUNT_ID, redirectResult.RouteValues["employerAccountId"]);
        }

        [Fact]
        public async Task DeletePost_CallsDeleteApprenticeshipEndpoint()
        {
            string expectedAction = "Index";

            var result = await Sut.DeletePost(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID, true);

           _mockFavouritesWriteRepository.Verify(v => v.DeleteApprenticeshipFavourites(It.IsAny<string>(),It.IsAny<string>()),Times.AtLeast(1));
        }

        [Fact]
        public async Task DeletePost_WithFalseConfirmation_ReturnsRedirectToActionResult()
        {
            string expectedAction = "Index";

            var result = await Sut.DeletePost(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID, false);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal(expectedAction, redirectResult.ActionName);
            Assert.Equal(EMPLOYER_ACCOUNT_ID, redirectResult.RouteValues["employerAccountId"]);
        }


    }
}
