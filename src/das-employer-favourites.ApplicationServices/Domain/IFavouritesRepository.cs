using System.Threading.Tasks;

namespace DfE.EmployerFavourites.ApplicationServices.Domain
{
    public interface IFavouritesRepository
    {
        Task<ApprenticeshipFavourites> GetApprenticeshipFavourites(string employerAccountId);
        Task SaveApprenticeshipFavourites(string employerAccountId, ApprenticeshipFavourites apprenticeshipFavourite);
    }
}