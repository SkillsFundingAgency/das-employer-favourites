using System.Collections.Generic;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Domain;
using ReadModel = DfE.EmployerFavourites.Domain.ReadModel;
using WriteModel = DfE.EmployerFavourites.Domain.WriteModel;

namespace DfE.EmployerFavourites.IntegrationTests.Stubs
{
    internal class StubFavouritesRepository : IFavouritesReadRepository, IFavouritesWriteRepository
    {
        public Task<ReadModel.ApprenticeshipFavourites> GetApprenticeshipFavourites(string employerAccountId)
        {
            var favourites = new ReadModel.ApprenticeshipFavourites
            {
                new ReadModel.ApprenticeshipFavourite
                {
                    ApprenticeshipId = "123",
                    Title = "Test Standard1",
                    Providers = new List<ReadModel.Provider>
                    {
                        new ReadModel.Provider { Name = "Test Provider", Ukprn = 10000020 }
                    }
                },
                new ReadModel.ApprenticeshipFavourite
                {
                    ApprenticeshipId = "456",
                    Title = "Test Standard2"
                },
                new ReadModel.ApprenticeshipFavourite
                {
                    ApprenticeshipId = "789",
                    Title = "Test Standard3",
                    Providers = new List<ReadModel.Provider>
                    {
                        new ReadModel.Provider { Name = "Test Provider", Ukprn = 10000020 }
                    }
                }
            };

            return Task.FromResult(favourites);
        }

        public Task SaveApprenticeshipFavourites(string employerAccountId, WriteModel.ApprenticeshipFavourites apprenticeshipFavourite)
        {
            return Task.CompletedTask;
        }
    }
}