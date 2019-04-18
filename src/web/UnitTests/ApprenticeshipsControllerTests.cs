using System;
using DfE.EmployerFavourites.Web.Controllers;
using EmployerFavouritesWeb.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DfE.EmployerFavourites.UnitTests
{
    public class ApprenticeshipsControllerTests
    {
        public const string TEST_MA_ACCOUNTS_HOME_URL = "https://ma-accounts-home.com/";
        private readonly Mock<IOptions<ExternalLinks>> _mockConfig;
        private readonly ApprenticeshipsController _sut;

        public ApprenticeshipsControllerTests()
        {
            _mockConfig = new Mock<IOptions<ExternalLinks>>();
            _mockConfig.Setup(x => x.Value).Returns(new ExternalLinks { AccountsHomePage = new Uri(TEST_MA_ACCOUNTS_HOME_URL) });

            _sut = new ApprenticeshipsController(Mock.Of<ILogger<ApprenticeshipsController>>(), _mockConfig.Object);
        }

        [Fact]
        public void Add_ReturnsRedirectResult_ForValidStandardId()
        {
            var result = _sut.Add("20");

            Assert.IsType<RedirectResult>(result);
        }

        [Theory]
        [InlineData("420-12-1")]
        [InlineData("420-1-12")]
        [InlineData("420-13-1")]
        [InlineData("420-13-13")]
        public void Add_ReturnsRedirectResult_ForValidFrameworkId(string frameworkId)
        {
            var result = _sut.Add(frameworkId);

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
        public void Add_ReturnsBadRequest_ForInvalidApprenticeshipId(string id)
        {
            var result = _sut.Add(id);

            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100000000)]
        [InlineData(9999999)]
        [InlineData(-10000023)]
        public void Add_ReturnsBadRequest_ForInvalidUkprn(int ukprn)
        {
            var result = _sut.Add("30", ukprn);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Add_ReturnRedirectUrl_FromConfig()
        {
            var result = _sut.Add("30", 12345678);

            var response = Assert.IsType<RedirectResult>(result);
            Assert.Equal(TEST_MA_ACCOUNTS_HOME_URL, response.Url);
        }
    }
}