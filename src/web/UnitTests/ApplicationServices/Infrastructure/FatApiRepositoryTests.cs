using System;
using System.Collections.Generic;
using System.Text;
using DfE.EmployerFavourites.ApplicationServices.Infrastructure;
using Moq;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Types;
using Xunit;

namespace DfE.EmployerFavourites.UnitTests.ApplicationServices.Infrastructure
{
    public class FatApiRepositoryTests
    {
        private Mock<IStandardApiClient> _mockStandardApiClient;
        private Mock<IFrameworkApiClient> _mockFrameworkApiClient;
        private FatApiRepository _sut;

        public FatApiRepositoryTests()
        {
            _mockFrameworkApiClient = new Mock<IFrameworkApiClient>();
            _mockStandardApiClient = new Mock<IStandardApiClient>();

            _mockFrameworkApiClient.Setup(s => s.Get("420-1-2")).Returns(new Framework() {Title = "Apprenticeship1"});
            _mockStandardApiClient.Setup(s => s.Get("123")).Returns(new Standard() {Title = "Apprenticeship2"});

            _sut = new FatApiRepository(_mockStandardApiClient.Object,_mockFrameworkApiClient.Object);
        }

        [Fact]
        public void WhenFrameworkID_ThenCallsFrameworkEndpoint()
        {
            var frameworkId = "420-1-2";
          var result =  _sut.GetApprenticeshipName(frameworkId);

          _mockFrameworkApiClient.Verify(s => s.Get(frameworkId),Times.Once);
          Assert.Equal("Apprenticeship1",result);
        }
        [Fact]
        public void WhenStandardID_ThenCallsStandardEndpoint()
        {
            var standardId = "123";
            var result = _sut.GetApprenticeshipName(standardId);

            _mockStandardApiClient.Verify(s => s.Get(standardId), Times.Once);
            Assert.Equal("Apprenticeship2", result);
        }
    }
}
