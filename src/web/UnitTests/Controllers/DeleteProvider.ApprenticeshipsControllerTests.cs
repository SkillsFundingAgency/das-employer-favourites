using System.Threading.Tasks;
using DfE.EmployerFavourites.Web.Application.Exceptions;
using DfE.EmployerFavourites.Web.Controllers;
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
        public async Task DeleteTrainingProvider_ReturnsBadRequest_ForInvalidEmployerAccountId(string id)
        {
            var result = await Sut.DeleteProvider(id,"123",12345);

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
        public async Task DeleteTrainingProvider_ReturnsBadRequest_ForInvalidApprenticeshipId(string id)
        {
            var result = await Sut.DeleteProvider("AB12XY", id, 12345);

            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task DeleteTrainingProvider_ReturnsBadRequest_ForInvalidUkprn(int ukprn)
        {
            var result = await Sut.DeleteProvider("AB12XY", "123", ukprn);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteTrainingProvider_ReturnsViewResult_WithProviderDetails()
        {
            var result = await Sut.DeleteProvider(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID,UKPRN);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<DeleteTrainingProviderViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task DeleteTrainingProvider_ThrowsException_WhenApprenticeshipNotInFavourites()
        {
            await Assert.ThrowsAsync<EntityNotFoundException>(() => Sut.DeleteProvider(EMPLOYER_ACCOUNT_ID, "66666",UKPRN));
        }

        [Fact]
        public async Task DeleteTrainingProvider_ThrowsException_WhenNoProvidersForApprenticeship()
        {
            await Assert.ThrowsAsync<EntityNotFoundException>(() => Sut.DeleteProvider(EMPLOYER_ACCOUNT_ID, "420-2-1", UKPRN));
        }

        [Fact]
        public async Task DeleteTrainingProvider_ThrowsException_WhenProviderForApprenticeshipNotInFavourites()
        {
            await Assert.ThrowsAsync<EntityNotFoundException>(() => Sut.DeleteProvider(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID, 17262546));
        }


    }
}
