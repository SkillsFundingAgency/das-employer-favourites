using System;
using System.IO;
using DfE.EmployerFavourites.ApplicationServices.Commands;
using DfE.EmployerFavourites.ApplicationServices.Domain;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace DfE.EmployerFavourites.UnitTests.ApplicationServices.Commands
{
    public class GetApprenticeshipFavouriteRequestHandlerTests
    {
        private readonly GetApprenticeshipFavouriteRequestHandler _sut;
        private readonly Mock<IEmployerAccountRepository> _mockEmployerAccountRepository;
        private readonly Mock<IFavouritesRepository> _mockFavouritesRepository;
        private readonly Mock<ILogger<GetApprenticeshipFavouriteRequestHandler>> _mockLogger;
        private string _employerAccountId = "XXX123";

        public GetApprenticeshipFavouriteRequestHandlerTests()
        {
            _mockLogger = new Mock<ILogger<GetApprenticeshipFavouriteRequestHandler>>();
            _mockEmployerAccountRepository = new Mock<IEmployerAccountRepository>();
            _mockFavouritesRepository = new Mock<IFavouritesRepository>();

            _mockFavouritesRepository.Setup(s => s.GetApprenticeshipFavourites(_employerAccountId))
                .ReturnsAsync(new ApprenticeshipFavourites());

            _sut = new GetApprenticeshipFavouriteRequestHandler(_mockLogger.Object, _mockFavouritesRepository.Object, _mockEmployerAccountRepository.Object);
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
        public async Task Handle_WhenValidUserId_ThenGetFavourites()
        {
            var result = await _sut.Handle(new GetApprenticeshipFavouritesRequest() { EmployerAccountID = _employerAccountId }, default);

           _mockFavouritesRepository.Verify(x => x.GetApprenticeshipFavourites(_employerAccountId),Times.Once);
        }

        [Fact]
        public async Task Handle_WhenValidUserId_ThenReturnsApprenticeFavourites()
        {
            var result = await _sut.Handle(new GetApprenticeshipFavouritesRequest() { EmployerAccountID = _employerAccountId }, default);

            Assert.IsType<ApprenticeshipFavourites>(result);
        }
    }
}
