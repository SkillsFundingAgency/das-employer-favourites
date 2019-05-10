using System;
using DfE.EmployerFavourites.ApplicationServices.Domain;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using DfE.EmployerFavourites.ApplicationServices.Infrastructure.Interfaces;
using DfE.EmployerFavourites.ApplicationServices.Queries;
using Xunit;

namespace DfE.EmployerFavourites.UnitTests.ApplicationServices.Commands
{
    public class GetApprenticeshipFavouriteRequestHandlerTests
    {
        private readonly GetApprenticeshipFavouriteRequestHandler _sut;
        private readonly Mock<IFavouritesRepository> _mockFavouritesRepository;
        private readonly Mock<ILogger<GetApprenticeshipFavouriteRequestHandler>> _mockLogger;
        private readonly Mock<IFatRepository> _mockFatRepository;
        private string _employerAccountId = "XXX123";
        private ApprenticeshipFavourites _apprenticeFavourites;

        public GetApprenticeshipFavouriteRequestHandlerTests()
        {
            _mockLogger = new Mock<ILogger<GetApprenticeshipFavouriteRequestHandler>>();
            _mockFavouritesRepository = new Mock<IFavouritesRepository>();
            _mockFatRepository = new Mock<IFatRepository>();

            _apprenticeFavourites = new ApprenticeshipFavourites() { new ApprenticeshipFavourite("123"),new ApprenticeshipFavourite("234")};


            _mockFavouritesRepository.Setup(s => s.GetApprenticeshipFavourites(_employerAccountId))
                .ReturnsAsync(_apprenticeFavourites);
            _mockFatRepository.Setup(s => s.GetApprenticeshipName("123")).Returns("apprenticeship1");
            _mockFatRepository.Setup(s => s.GetApprenticeshipName("234")).Returns("apprenticeship2");

            _sut = new GetApprenticeshipFavouriteRequestHandler(_mockLogger.Object, _mockFavouritesRepository.Object, _mockFatRepository.Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Handle_WhenInvalidEmployerAccountId_ThenArgumentException(string employerAccountId)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.Handle(new GetApprenticeshipFavouritesRequest(){EmployerAccountID = employerAccountId},default ));
        }

        [Fact]
        public async Task Handle_WhenValidEmployerAccountId_ThenGetFavourites()
        {
            var result = await _sut.Handle(new GetApprenticeshipFavouritesRequest() { EmployerAccountID = _employerAccountId }, default);

           _mockFavouritesRepository.Verify(x => x.GetApprenticeshipFavourites(_employerAccountId),Times.Once);
        }

        [Fact]
        public async Task Handle_WhenValidEmployerAccountId_ThenReturnsApprenticeFavourites()
        {
            var result = await _sut.Handle(new GetApprenticeshipFavouritesRequest() { EmployerAccountID = _employerAccountId }, default);

            Assert.IsType<ApprenticeshipFavourites>(result);
        }

        [Fact]
        public async Task Handle_WhenValidEmployerAccountId_ThenGetApprenticeshipNames()
        {
            var result = await _sut.Handle(new GetApprenticeshipFavouritesRequest() { EmployerAccountID = _employerAccountId }, default);

            _mockFatRepository.Verify(s => s.GetApprenticeshipName(It.IsAny<string>()),Times.Exactly(2));
        }

        [Fact]
        public async Task Handle_WhenValidEmployerAccountId_ThenApprenticeshipNamesPopulated()
        {
            var result = await _sut.Handle(new GetApprenticeshipFavouritesRequest() { EmployerAccountID = _employerAccountId }, default);

            Assert.Equal("apprenticeship1",result[0].Title);
            Assert.Equal("apprenticeship2", result[1].Title);
        }
    }
}
