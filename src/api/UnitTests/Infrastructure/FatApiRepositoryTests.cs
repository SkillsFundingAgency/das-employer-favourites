using System.Threading.Tasks;
using DfE.EmployerFavourites.Api.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Providers.Api.Client;
using Xunit;

namespace DfE.EmployerFavourites.Api.UnitTests.Infrastructure
{
    public class FatApiRepositoryTests
    {
        private readonly Mock<IStandardApiClient> _mockStandardApiClient;
        private readonly Mock<IFrameworkApiClient> _mockFrameworkApiClient;
        private readonly Mock<IProviderApiClient> _mockProviderApiClient;
        private readonly FatApiRepository _sut;

        public FatApiRepositoryTests()
        {
            _mockFrameworkApiClient = new Mock<IFrameworkApiClient>();
            _mockStandardApiClient = new Mock<IStandardApiClient>();
            _mockProviderApiClient = new Mock<IProviderApiClient>();

            _mockFrameworkApiClient.Setup(s => s.GetAsync("420-1-2")).ReturnsAsync(new Framework { Title = "Apprenticeship1" });
            _mockStandardApiClient.Setup(s => s.GetAsync("123")).ReturnsAsync(new Standard { Title = "Apprenticeship2" });

            _sut = new FatApiRepository(_mockStandardApiClient.Object,_mockFrameworkApiClient.Object, _mockProviderApiClient.Object, Mock.Of<ILogger<FatApiRepository>>());
        }

        [Fact]
        public async Task WhenFrameworkID_ThenCallsFrameworkEndpoint()
        {
            var frameworkId = "420-1-2";
          var result = await _sut.GetApprenticeshipNameAsync(frameworkId);

          _mockFrameworkApiClient.Verify(s => s.GetAsync(frameworkId),Times.Once);
          Assert.Equal("Apprenticeship1",result);
        }

        [Fact]
        public async Task WhenStandardID_ThenCallsStandardEndpoint()
        {
            var standardId = "123";
            var result = await _sut.GetApprenticeshipNameAsync(standardId);

            _mockStandardApiClient.Verify(s => s.GetAsync(standardId), Times.Once);
            Assert.Equal("Apprenticeship2", result);
        }
    }
}
