using System.Linq;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DfE.EmployerFavourites.Web.UnitTests.Controllers
{
    //
    // See ApprenticeshipsControllerTestsBase.GetTestRepositoryFavourites() for Favouriites repository contents
    //
    public partial class ApprenticeshipsControllerTests : ApprenticeshipsControllerTestsBase
    {
        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfApprenticeships()
        {
            var result = await Sut.Index(EMPLOYER_ACCOUNT_ID);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ApprenticeshipFavouritesViewModel>(viewResult.ViewData.Model);
            Assert.Equal(5, model.Items.Count());
        }

        [Fact]
        public async Task Index_ReturnsModel_WithEmployerAccountId()
        {
            var result = await Sut.Index(EMPLOYER_ACCOUNT_ID);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.ViewData.Model as ApprenticeshipFavouritesViewModel;

            Assert.Equal(EMPLOYER_ACCOUNT_ID, model.EmployerAccountId);
        }

        [Fact]
        public async Task Index_ReturnsModel_ItemsIndicateProgrammeType()
        {
            var result = await Sut.Index(EMPLOYER_ACCOUNT_ID);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.ViewData.Model as ApprenticeshipFavouritesViewModel;

            Assert.True(model.Items.Single(x => x.Id == "420-2-1").IsFramework);
            Assert.False(model.Items.Single(x => x.Id == "30").IsFramework);
        }

        [Fact]
        public async Task Index_ReturnsModel_ItemsContainTitle()
        {
            var result = await Sut.Index(EMPLOYER_ACCOUNT_ID);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.ViewData.Model as ApprenticeshipFavouritesViewModel;

            Assert.Equal("Framework-420-2-1", model.Items.Single(x => x.Id == "420-2-1").Title);
            Assert.Equal("Standard-30", model.Items.Single(x => x.Id == "30").Title);
        }

        [Fact]
        public async Task Index_ReturnsModel_ItemsContainLevel()
        {
            var result = await Sut.Index(EMPLOYER_ACCOUNT_ID);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.ViewData.Model as ApprenticeshipFavouritesViewModel;

            Assert.Equal("3 (equivalent to A levels at grades A to E)", model.Items.Single(x => x.Id == "420-2-1").Level);
            Assert.Equal("4 (equivalent to certificate of higher education)", model.Items.Single(x => x.Id == "30").Level);
        }

        [Fact]
        public async Task Index_ReturnsModel_ItemsContainTypicalLength()
        {
            var result = await Sut.Index(EMPLOYER_ACCOUNT_ID);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.ViewData.Model as ApprenticeshipFavouritesViewModel;

            Assert.Equal("18 months", model.Items.Single(x => x.Id == "420-2-1").TypicalLength);
            Assert.Equal("24 months", model.Items.Single(x => x.Id == "30").TypicalLength);
        }

        [Fact]
        public async Task Index_ReturnsModel_FrameworkItemsContainExpiryDateValue()
        {
            var result = await Sut.Index(EMPLOYER_ACCOUNT_ID);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.ViewData.Model as ApprenticeshipFavouritesViewModel;

            Assert.Equal("2 January 2020", model.Items.Single(x => x.Id == "420-2-1").ExpiryDate); // Should show the date when no new start can be made i.e. expiry + 1 day
            Assert.Null(model.Items.Single(x => x.Id == "30").ExpiryDate);
        }

        [Fact]
        public async Task Index_ReturnsModel_WithUrlToApprenticehipOnFATWebsite()
        {
            var result = await Sut.Index(EMPLOYER_ACCOUNT_ID);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.ViewData.Model as ApprenticeshipFavouritesViewModel;

            Assert.Equal("https://fat-website/Apprenticeship/Apprenticeship/Framework/420-2-1", model.Items.Single(x => x.Id == "420-2-1").FatUrl.ToString()); 
            Assert.Equal("https://fat-website/Apprenticeship/Apprenticeship/Standard/30", model.Items.Single(x => x.Id == "30").FatUrl.ToString()); 
        }

        [Fact]
        public async Task Index_ReturnsModel_ItemsIndicatesIfItHasProviders()
        {
            var result = await Sut.Index(EMPLOYER_ACCOUNT_ID);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.ViewData.Model as ApprenticeshipFavouritesViewModel;

            Assert.False(model.Items.Single(x => x.Id == "420-2-1").HasTrainingProviders);
            Assert.True(model.Items.Single(x => x.Id == "70").HasTrainingProviders);
        }
    }
}
