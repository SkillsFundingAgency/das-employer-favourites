﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Api.Application.Commands;
using DfE.EmployerFavourites.Api.Controllers;
using DfE.EmployerFavourites.Api.Domain;
using DfE.EmployerFavourites.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using ReadModel = DfE.EmployerFavourites.Api.Domain.ReadModel;
using WriteModel = DfE.EmployerFavourites.Api.Domain.WriteModel;

namespace DfE.EmployerFavourites.Api.UnitTests.Controllers
{
    public class ApprenticeshipsControllerTests
    {
        private const string EmployerAccountIdNewList = "ABC123";
        private const string EmployerAccountIdExistingList = "XYZ123";
        private readonly ApprenticeshipsController _sut;
        private readonly Mock<IFavouritesWriteRepository> _mockWriteRepository = new Mock<IFavouritesWriteRepository>();
        private readonly Mock<IFavouritesReadRepository> _mockReadRepository = new Mock<IFavouritesReadRepository>();

        public ApprenticeshipsControllerTests()
        {
            ServiceProvider provider = BuildDependencies();
            var mediator = provider.GetService<IMediator>();

            var existingFavourites = new ReadModel.ApprenticeshipFavourites
            {
                new ReadModel.ApprenticeshipFavourite
                {
                    ApprenticeshipId = "55",
                    Providers = new List<ReadModel.Provider> { new ReadModel.Provider { Ukprn = 10000055 } }
                }
            };

            _mockReadRepository.Setup(x => x.GetApprenticeshipFavourites(EmployerAccountIdNewList)).ReturnsAsync(new ReadModel.ApprenticeshipFavourites());
            _mockReadRepository.Setup(x => x.GetApprenticeshipFavourites(EmployerAccountIdExistingList)).ReturnsAsync(existingFavourites);

            _sut = new ApprenticeshipsController(new NullLogger<ApprenticeshipsController>(), mediator);
        }

        [Fact]
        public async Task Put_ReturnsCreatedResult_WhenNewListCreatedForEmployerAccount()
        {
            var favourites = new List<Favourite>
            {
                new Favourite { ApprenticeshipId = "50", Ukprns = new List<int> { 10000020 } }
            };

            var result = await _sut.Put(EmployerAccountIdNewList, favourites);
            
            Assert.IsAssignableFrom<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Put_ReturnsNoContentResult_WhenNewListCreatedEmployerAccountWithExistingList()
        {
            var favourites = new List<Favourite>
            {
                new Favourite { ApprenticeshipId = "50", Ukprns = new List<int> { 10000020 } }
            };

            var result = await _sut.Put(EmployerAccountIdExistingList, favourites);

            Assert.IsAssignableFrom<NoContentResult>(result);
        }

        [Fact]
        public async Task Put_ReturnsNoContentResult_WhenAddingNewProviderToExistingApprenticeship()
        {
            var favourites = new List<Favourite>
            {
                new Favourite { ApprenticeshipId = "55", Ukprns = new List<int> { 10000030 } }
            };

            var result = await _sut.Put(EmployerAccountIdExistingList, favourites);

            Assert.IsAssignableFrom<NoContentResult>(result);
        }

        [Fact]
        public async Task Put_ReplacesExistingListInRepo_WithNewList()
        {
            var favourites = new List<Favourite>
            {
                new Favourite { ApprenticeshipId = "60", Ukprns = new List<int> { 10000020 } }
            };

            var result = await _sut.Put(EmployerAccountIdExistingList, favourites);

            _mockWriteRepository.Verify(x => x.SaveApprenticeshipFavourites(EmployerAccountIdExistingList, Matchers.ContainsOnly("60", 10000020)));
        }

        private ServiceProvider BuildDependencies()
        {
            var services = new ServiceCollection();
            services.AddMediatR(typeof(Startup).Assembly);
            services.AddScoped<IFavouritesWriteRepository>(x => _mockWriteRepository.Object);
            services.AddScoped<IFavouritesReadRepository>(x => _mockReadRepository.Object);
            services.AddScoped<ILogger<SaveApprenticeshipFavouriteCommandHandler>>(x => new NullLogger<SaveApprenticeshipFavouriteCommandHandler>());
            var provider = services.BuildServiceProvider();

            return provider;
        }

        private static class Matchers
        {
            internal static WriteModel.ApprenticeshipFavourites ContainsOnly(string apprenticeshipId, int ukprn)
            {
                return Match.Create<WriteModel.ApprenticeshipFavourites>(s =>
                {
                    if (s == null || s.Count != 1)
                        return false;

                    var item = s.First();

                    if (item.ApprenticeshipId != apprenticeshipId || item.Ukprns.First() != ukprn)
                        return false;

                    return true;
                });
            }
        }
    }
}
