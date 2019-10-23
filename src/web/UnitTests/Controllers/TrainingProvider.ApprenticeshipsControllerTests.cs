using System.Linq;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using DfE.EmployerFavourites.Web.Application.Exceptions;

namespace DfE.EmployerFavourites.Web.UnitTests.Controllers
{
    //
    // See ApprenticeshipsControllerTestsBase.GetTestRepositoryFavourites() for Favouriites repository contents
    //
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
        public async Task TrainingProvider_ReturnsBadRequest_ForInvalidEmployerAccountId(string id)
        {
            var result = await Sut.Index(id);

            Assert.IsType<BadRequestResult>(result);
        }
                
        [Fact]
        public async Task TrainingProvider_ReturnsViewResult_WithProviderDetails()
        {
            var result = await Sut.TrainingProvider(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<TrainingProvidersViewModel>(viewResult.ViewData.Model);
        }

       [Fact]
       public async Task TrainingProvider_ReturnsModel_WithNameOfTrainingProvider()
       {
           var result = await Sut.TrainingProvider(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID);

           var viewResult = Assert.IsType<ViewResult>(result);
           var model = viewResult.ViewData.Model as TrainingProvidersViewModel;

           Assert.Equal("Test Provider Ltd", model.Items[0].ProviderName);
       }

        [Fact]
        public async Task TrainingProvider_ReturnsModel_WithPhoneNumber()
        {
            var result = await Sut.TrainingProvider(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.ViewData.Model as TrainingProvidersViewModel;

            Assert.Equal("020 123 1234", model.Items[0].Phone);
        }

        [Fact]
        public async Task TrainingProvider_ReturnsModel_WithEmail()
        {
            var result = await Sut.TrainingProvider(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.ViewData.Model as TrainingProvidersViewModel;

            Assert.Equal("test@test.com", model.Items[0].Email);
        }

        [Fact]
        public async Task TrainingProvider_ReturnsModel_WithWebsite()
        {
            var result = await Sut.TrainingProvider(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.ViewData.Model as TrainingProvidersViewModel;

            Assert.Equal("https://www.testprovider.com/", model.Items[0].Website);
        }

        [Fact]
        public async Task TrainingProvider_ReturnsModel_WithUrlToProviderPageOnFATWebsite()
        {
            var result = await Sut.TrainingProvider(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.ViewData.Model as TrainingProvidersViewModel;

            Assert.Equal("https://fat-website/Providers/10000020", model.Items.Single(x => x.Ukprn == 10000020).FatUrl?.ToString());
        }

        [Fact]
        public async Task TrainingProvider_ReturnsModel_WithNotDataAvailableWhenNoValuesReturnedFromRepo()
        {
            var result = await Sut.TrainingProvider(EMPLOYER_ACCOUNT_ID, APPRENTICESHIPID_NO_PROVIDER_DATA);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.ViewData.Model as TrainingProvidersViewModel;

            Assert.Equal("no data available", model.Items[0].Email);
            Assert.Equal("no data available", model.Items[0].EmployerSatisfaction);
            Assert.Equal("no data available", model.Items[0].LearnerSatisfaction);
            Assert.Equal("no data available", model.Items[0].Phone);
            Assert.Equal("no data available", model.Items[0].Website);
        }

        [Fact]
        public async Task TrainingProvider_ThrowsException_WhenApprenticeshipNotInFavourites()
        {
            await Assert.ThrowsAsync<EntityNotFoundException>(() => Sut.TrainingProvider(EMPLOYER_ACCOUNT_ID, "66666"));
        }

        [Fact]
        public async Task TrainingProvider_RedirectsToIndex_WhenNoProvidersForApprenticeship()
        {
            var result = await Sut.TrainingProvider(EMPLOYER_ACCOUNT_ID, "420-2-1");

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index",redirectResult.ActionName);
            Assert.Equal(EMPLOYER_ACCOUNT_ID,redirectResult.RouteValues["employerAccountId"]);
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
            var result = await Sut.TrainingProvider(EMPLOYER_ACCOUNT_ID, id);

            Assert.IsType<BadRequestResult>(result);
        }
    }
}
