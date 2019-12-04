using System.Collections.Generic;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Api.Domain;
using DfE.EmployerFavourites.Api.Domain.ReadModel;

namespace DfE.EmployerFavourites.Api.IntegrationTests.Stubs
{
    public class StubFavouritesRepository : IFavouritesReadRepository, IFavouritesWriteRepository
    {
        public Task<Domain.ReadModel.ApprenticeshipFavourites> GetApprenticeshipFavourites(string employerAccountId)
        {
            if (employerAccountId == "ABC123")
                return Task.FromResult(new Domain.ReadModel.ApprenticeshipFavourites());

            var result = new Domain.ReadModel.ApprenticeshipFavourites
            {
                new Domain.ReadModel.ApprenticeshipFavourite()
                {
                    ApprenticeshipId = "123-1-2",
                    Providers = new List<Provider>()
                    {
                        new Provider(){ Name = "Provider 1", Ukprn = 12345678 },
                        new Provider() { Name = "Provider 2", Ukprn= 23456789 }
                    }
                }
            };

            return Task.FromResult(result);
        }

        public Task SaveApprenticeshipFavourites(string employerAccountId, Domain.WriteModel.ApprenticeshipFavourites apprenticeshipFavourite)
        {
            return Task.CompletedTask;
        }
    }
}
