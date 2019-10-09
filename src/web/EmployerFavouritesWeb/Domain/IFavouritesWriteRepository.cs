using System.Threading.Tasks;

namespace DfE.EmployerFavourites.Domain
{
    public interface IFavouritesWriteRepository
    {
        Task SaveApprenticeshipFavourites(string employerAccountId, WriteModel.ApprenticeshipFavourites apprenticeshipFavourites);
        Task DeleteApprenticeshipFavourites(string employerAccountId, string apprenticeshipId);

        Task DeleteApprenticeshipProviderFavourites(string employerAccountId, string apprenticeshipId,
            int ukprn);
    }
}