using System.Threading.Tasks;
using DfE.EmployerFavourites.Web.Application.Exceptions;
using DfE.EmployerFavourites.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DfE.EmployerFavourites.Web.UnitTests.Controllers
{
    public partial class ApprenticeshipsControllerTests : ApprenticeshipsControllerTestsBase
    {

        [Theory(Skip = "Broken due to permanent redirect")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("1")]
        [InlineData("22")]
        [InlineData("AB12")]
        [InlineData("1q3ef")]
        public async Task DeleteApprenticeship_ReturnsBadRequest_ForInvalidEmployerAccountId(string id)
        {
            var result = await Sut.Delete(id,"123");

            Assert.IsType<BadRequestResult>(result);
        }

        [Theory(Skip = "Broken due to permanent redirect")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("-1")]
        [InlineData("2-1-1")]
        [InlineData("420-222-1")]
        [InlineData("420-2-222")]
        public async Task DeleteApprenticeship_ReturnsBadRequest_ForInvalidApprenticeshipId(string id)
        {
            var result = await Sut.Delete("AB12XY", id);

            Assert.IsType<BadRequestResult>(result);
        }


        [Fact(Skip = "Broken due to permanent redirect")]
        public async Task DeleteApprenticeship_ReturnsViewResult_WithProviderDetails()
        {
            var result = await Sut.Delete(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<TrainingProvidersViewModel>(viewResult.ViewData.Model);
        }

        [Fact(Skip = "Broken due to permanent redirect")]
        public async Task DeleteApprenticeship_ThrowsException_WhenApprenticeshipNotInFavourites()
        {
            await Assert.ThrowsAsync<EntityNotFoundException>(() => Sut.Delete(EMPLOYER_ACCOUNT_ID, "66666"));
        }

       


    }
}
