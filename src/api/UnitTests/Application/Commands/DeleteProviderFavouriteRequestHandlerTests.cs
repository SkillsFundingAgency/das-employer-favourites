using System;
using System.Collections.Generic;
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
    public class DeleteProviderFavouriteCommandHandlerTests
    {
        private readonly DeleteProviderFavouriteCommandHandler _sut;
        private readonly Mock<IFavouritesReadRepository> _mockFavouritesRepository;
        private readonly Mock<IFavouritesWriteRepository> _mockWriteFavouritesRepository;
        private readonly Mock<ILogger<DeleteProviderFavouriteCommandHandler>> _mockLogger;
        private readonly string _employerAccountId = "XXX123";
        private readonly string _apprenticeshipId = "123";
        private readonly int _ukprn = 123456;
        private ReadModel.ApprenticeshipFavourites _apprenticeFavourites;

        public DeleteProviderFavouriteCommandHandlerTests()
        {
            _mockLogger = new Mock<ILogger<DeleteProviderFavouriteCommandHandler>>();
            _mockFavouritesRepository = new Mock<IFavouritesReadRepository>();
            _mockWriteFavouritesRepository = new Mock<IFavouritesWriteRepository>();

            _apprenticeFavourites = new ReadModel.ApprenticeshipFavourites
            {
                new ReadModel.ApprenticeshipFavourite("123", "apprenticeship1")
                {
                    Providers = new List<ReadModel.Provider>()
                    {
                        new ReadModel.Provider()
                        {
                           Name = "Provider 1",
                            Ukprn = 123456
                        },
                        new ReadModel.Provider()
                        {
                            Name = "Provider 2",
                            Ukprn = 234567
                        }
                    }
                },
                new ReadModel.ApprenticeshipFavourite("234", "apprenticeship2")
                {
                    Providers = new List<ReadModel.Provider>()
                    {
                        new ReadModel.Provider()
                        {
                            Name = "Provider 1",
                            Ukprn = 123456
                        },
                        new ReadModel.Provider()
                        {
                            Name = "Provider 2",
                            Ukprn = 234567
                        }
                    }
                }
            };


            _mockFavouritesRepository.Setup(s => s.GetApprenticeshipFavourites(_employerAccountId))
                .ReturnsAsync(_apprenticeFavourites);

            _sut = new DeleteProviderFavouriteCommandHandler(_mockWriteFavouritesRepository.Object, _mockFavouritesRepository.Object,_mockLogger.Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Handle_WhenInvalidEmployerAccountId_ThenArgumentException(string employerAccountId)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.Handle(new DeleteProviderFavouriteCommand() { EmployerAccountId = employerAccountId, ApprenticeshipId = _apprenticeshipId}, default));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Handle_WhenInvalidApprenticeshipId_ThenArgumentException(string apprenticeshipId)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.Handle(new DeleteProviderFavouriteCommand() { EmployerAccountId = _employerAccountId, ApprenticeshipId = apprenticeshipId}, default));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Handle_WhenInvalidUkprn_ThenArgumentException(int ukprn)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.Handle(new DeleteProviderFavouriteCommand() { EmployerAccountId = _employerAccountId, ApprenticeshipId = _apprenticeshipId,Ukprn = ukprn}, default));
        }

        [Fact]
        public async Task Handle_WhenValidEmployerAccountIdAndApprenticeshipIdAndUkprn_ThenGetFavourites()
        {
            var result = await _sut.Handle(new DeleteProviderFavouriteCommand { EmployerAccountId = _employerAccountId, ApprenticeshipId = _apprenticeshipId, Ukprn = _ukprn}, default);

           _mockFavouritesRepository.Verify(x => x.GetApprenticeshipFavourites(_employerAccountId),Times.Once);
        }

        [Fact]
        public async Task Handle_WhenValid_ThenApprenticeshipDeleted()
        {
            var result = await _sut.Handle(new DeleteProviderFavouriteCommand { EmployerAccountId = _employerAccountId, ApprenticeshipId = _apprenticeshipId, Ukprn = _ukprn }, default);

            Assert.IsType<DeleteProviderFavouriteCommandResponse>(result);

            Assert.Equal(DomainUpdateStatus.Deleted,result.CommandResult);
        }

        [Fact]
        public async Task Handle_WhenValid_ThenBasketUpdated()
        {
            var result = await _sut.Handle(new DeleteProviderFavouriteCommand { EmployerAccountId = _employerAccountId, ApprenticeshipId = _apprenticeshipId, Ukprn = _ukprn }, default);

          _mockWriteFavouritesRepository.Verify(s => s.SaveApprenticeshipFavourites(_employerAccountId,It.IsAny<ApprenticeshipFavourites>()),Times.Once);
        }


    }
}
