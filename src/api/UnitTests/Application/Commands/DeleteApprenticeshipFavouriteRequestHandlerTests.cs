using System;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Api.Application.Commands;
using DfE.EmployerFavourites.Api.Domain;
using DfE.EmployerFavourites.Api.Domain.WriteModel;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using ReadModel = DfE.EmployerFavourites.Api.Domain.ReadModel;

namespace DfE.EmployerFavourites.Api.UnitTests.Application.Commands
{
    public class DeleteApprenticeshipFavouriteCommandHandlerTests
    {
        private readonly DeleteApprenticeshipFavouriteCommandHandler _sut;
        private readonly Mock<IFavouritesReadRepository> _mockFavouritesRepository;
        private readonly Mock<IFavouritesWriteRepository> _mockWriteFavouritesRepository;
        private readonly Mock<ILogger<DeleteApprenticeshipFavouriteCommandHandler>> _mockLogger;
        private readonly string _employerAccountId = "XXX123";
        private readonly string _apprenticeshipId = "123";
        private ReadModel.ApprenticeshipFavourites _apprenticeFavourites;

        public DeleteApprenticeshipFavouriteCommandHandlerTests()
        {
            _mockLogger = new Mock<ILogger<DeleteApprenticeshipFavouriteCommandHandler>>();
            _mockFavouritesRepository = new Mock<IFavouritesReadRepository>();
            _mockWriteFavouritesRepository = new Mock<IFavouritesWriteRepository>();

            _apprenticeFavourites = new ReadModel.ApprenticeshipFavourites { new ReadModel.ApprenticeshipFavourite("123", "apprenticeship1"), new ReadModel.ApprenticeshipFavourite("234", "apprenticeship2") };


            _mockFavouritesRepository.Setup(s => s.GetApprenticeshipFavourites(_employerAccountId))
                .ReturnsAsync(_apprenticeFavourites);

            _sut = new DeleteApprenticeshipFavouriteCommandHandler(_mockWriteFavouritesRepository.Object, _mockFavouritesRepository.Object,_mockLogger.Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Handle_WhenInvalidEmployerAccountId_ThenArgumentException(string employerAccountId)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.Handle(new DeleteApprenticeshipFavouriteCommand() { EmployerAccountId = employerAccountId , ApprenticeshipId = _apprenticeshipId}, default));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Handle_WhenInvalidApprenticeshipId_ThenArgumentException(string apprenticeshipId)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.Handle(new DeleteApprenticeshipFavouriteCommand() { EmployerAccountId = _employerAccountId, ApprenticeshipId = apprenticeshipId}, default));
        }

        [Fact]
        public async Task Handle_WhenValidEmployerAccountIdAndApprenticeshipId_ThenGetFavourites()
        {
            var result = await _sut.Handle(new DeleteApprenticeshipFavouriteCommand { EmployerAccountId = _employerAccountId, ApprenticeshipId = _apprenticeshipId}, default);

           _mockFavouritesRepository.Verify(x => x.GetApprenticeshipFavourites(_employerAccountId),Times.Once);
        }

        [Fact]
        public async Task Handle_WhenValid_ThenApprenticeshipDeleted()
        {
            var result = await _sut.Handle(new DeleteApprenticeshipFavouriteCommand { EmployerAccountId = _employerAccountId, ApprenticeshipId = _apprenticeshipId}, default);

            Assert.IsType<DeleteApprenticeshipFavouriteCommandResponse>(result);

            Assert.Equal(DomainUpdateStatus.Deleted,result.CommandResult);
        }

        [Fact]
        public async Task Handle_WhenValid_ThenBasketUpdated()
        {
            var result = await _sut.Handle(new DeleteApprenticeshipFavouriteCommand { EmployerAccountId = _employerAccountId, ApprenticeshipId = _apprenticeshipId}, default);

          _mockWriteFavouritesRepository.Verify(s => s.SaveApprenticeshipFavourites(_employerAccountId,It.IsAny<ApprenticeshipFavourites>()),Times.Once);
        }


    }
}
