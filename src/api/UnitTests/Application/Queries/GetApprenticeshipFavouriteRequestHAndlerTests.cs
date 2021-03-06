﻿using System;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;
using DfE.EmployerFavourites.Api.Application.Queries;
using ReadModel = DfE.EmployerFavourites.Api.Domain.ReadModel;
using DfE.EmployerFavourites.Api.Domain;

namespace DfE.EmployerFavourites.Api.UnitTests.ApplicationServices.Commands
{
    public class GetApprenticeshipFavouriteRequestHandlerTests
    {
        private readonly GetApprenticeshipFavouriteRequestHandler _sut;
        private readonly Mock<IFavouritesReadRepository> _mockFavouritesRepository;
        private readonly Mock<ILogger<GetApprenticeshipFavouriteRequestHandler>> _mockLogger;
        private readonly string _employerAccountId = "XXX123";
        private ReadModel.ApprenticeshipFavourites _apprenticeFavourites;

        public GetApprenticeshipFavouriteRequestHandlerTests()
        {
            _mockLogger = new Mock<ILogger<GetApprenticeshipFavouriteRequestHandler>>();
            _mockFavouritesRepository = new Mock<IFavouritesReadRepository>();

            _apprenticeFavourites = new ReadModel.ApprenticeshipFavourites { new ReadModel.ApprenticeshipFavourite("123", "apprenticeship1"), new ReadModel.ApprenticeshipFavourite("234", "apprenticeship2") };


            _mockFavouritesRepository.Setup(s => s.GetApprenticeshipFavourites(_employerAccountId))
                .ReturnsAsync(_apprenticeFavourites);

            _sut = new GetApprenticeshipFavouriteRequestHandler(_mockLogger.Object, _mockFavouritesRepository.Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Handle_WhenInvalidEmployerAccountId_ThenArgumentException(string employerAccountId)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.Handle(new GetApprenticeshipFavouritesRequest { EmployerAccountId = employerAccountId }, default));
        }

        [Fact]
        public async Task Handle_WhenValidEmployerAccountId_ThenGetFavourites()
        {
            var result = await _sut.Handle(new GetApprenticeshipFavouritesRequest { EmployerAccountId = _employerAccountId }, default);

           _mockFavouritesRepository.Verify(x => x.GetApprenticeshipFavourites(_employerAccountId),Times.Once);
        }

        [Fact]
        public async Task Handle_WhenValidEmployerAccountId_ThenReturnsApprenticeFavourites()
        {
            var result = await _sut.Handle(new GetApprenticeshipFavouritesRequest { EmployerAccountId = _employerAccountId }, default);

            Assert.IsType<ReadModel.ApprenticeshipFavourites>(result);
        }

        [Fact]
        public async Task Handle_WhenValidEmployerAccountId_ThenApprenticeshipNamesPopulated()
        {
            var result = await _sut.Handle(new GetApprenticeshipFavouritesRequest { EmployerAccountId = _employerAccountId }, default);

            Assert.Equal("apprenticeship1",result[0].Title);
            Assert.Equal("apprenticeship2", result[1].Title);
        }
    }
}
