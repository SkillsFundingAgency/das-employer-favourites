using System.Threading.Tasks;

namespace DfE.EmployerFavourites.Domain
{
    public interface IFavouritesReadRepository
    {
        Task<ReadModel.ApprenticeshipFavourites> GetApprenticeshipFavourites(string employerAccountId);
    }
}