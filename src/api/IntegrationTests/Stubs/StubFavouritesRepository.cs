using System.Threading.Tasks;
using DfE.EmployerFavourites.Api.Domain;

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
            };

            return Task.FromResult(result);
        }

        public Task SaveApprenticeshipFavourites(string employerAccountId, Domain.WriteModel.ApprenticeshipFavourites apprenticeshipFavourite)
        {
            return Task.CompletedTask;
        }
    }
}
