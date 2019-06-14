using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Api.Controllers;
using DfE.EmployerFavourites.Api.Domain;
using DfE.EmployerFavourites.Api.Domain.WriteModel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
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
            ServiceProvider provider = BuildDependencies();
            var mediator = provider.GetService<IMediator>();

            var result = await _sut.Put(EmployerAccountIdNewList, "50", 10000020);
            
            Assert.IsAssignableFrom<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Put_ReturnsNoContentResult_WhenNewListCreatedEmployerAccountWithExistingList()
        {
            ServiceProvider provider = BuildDependencies();
            var mediator = provider.GetService<IMediator>();

            var result = await _sut.Put(EmployerAccountIdExistingList, "50", 10000020);

            Assert.IsAssignableFrom<NoContentResult>(result);
        }


        private static class Matchers
        {
            internal static WriteModel.ApprenticeshipFavourites ContainsJustNewItem(string apprenticeshipId, int ukprn)
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

            internal static ApprenticeshipFavourites AddedToExistingList(string apprenticeshipId, int ukprn)
            {
                return Match.Create<WriteModel.ApprenticeshipFavourites>(s =>
                {
                    if (s == null || s.Count == 1)
                        return false;

                    if (!s.Any(x => x.ApprenticeshipId == apprenticeshipId && x.Ukprns.First() == ukprn))
                        return false;

                    return true;
                });
            }

            internal static ApprenticeshipFavourites AddedUkprnToExistingApprenticeship(int ukprn)
            {
                return Match.Create<WriteModel.ApprenticeshipFavourites>(s =>
                {
                    if (s == null || s.Count != 1)
                        return false;

                    var providers = s.First().Ukprns;

                    if (providers.Count != 2 || !providers.Any(x => x == ukprn))
                        return false;

                    return true;
                });
            }
        }

        [Fact]
        public async Task Put_CreatesNewListInRepo_ForFirstSaveForEmployerAccountId()
        {
            ServiceProvider provider = BuildDependencies();
            var mediator = provider.GetService<IMediator>();

            var result = await _sut.Put(EmployerAccountIdNewList, "50", 10000020);

            _mockWriteRepository.Verify(x => x.SaveApprenticeshipFavourites(EmployerAccountIdNewList, Matchers.ContainsJustNewItem("50", 10000020)));
        }

        [Fact]
        public async Task Put_AddsFavouriteToExistingListInRepo_WhenNewListAndNewApprenticeship()
        {
            ServiceProvider provider = BuildDependencies();
            var mediator = provider.GetService<IMediator>();

            var result = await _sut.Put(EmployerAccountIdExistingList, "60", 10000020);

            _mockWriteRepository.Verify(x => x.SaveApprenticeshipFavourites(EmployerAccountIdExistingList, Matchers.AddedToExistingList("60", 10000020)));
        }

        [Fact]
        public async Task Put_AddsProviderToExistingListInRepo_WhenApprenticeshipAlreadyExists()
        {
            ServiceProvider provider = BuildDependencies();
            var mediator = provider.GetService<IMediator>();

            var result = await _sut.Put(EmployerAccountIdExistingList, "55", 10000020);

            _mockWriteRepository.Verify(x => x.SaveApprenticeshipFavourites(EmployerAccountIdExistingList, Matchers.AddedUkprnToExistingApprenticeship(10000020)));
        }

        private ServiceProvider BuildDependencies()
        {
            var services = new ServiceCollection();
            services.AddMediatR(typeof(Startup).Assembly);
            services.AddScoped<IFavouritesWriteRepository>(x => _mockWriteRepository.Object);
            services.AddScoped<IFavouritesReadRepository>(x => _mockReadRepository.Object);
            var provider = services.BuildServiceProvider();

            return provider;
        }
    }
}
