using System.Threading.Tasks;
using DfE.EmployerFavourites.ApplicationServices.Domain.ReadModel;

namespace DfE.EmployerFavourites.ApplicationServices.Domain
{
    public interface IFavouritesReadRepository
    {
        Task<ApprenticeshipFavourites> GetApprenticeshipFavourites(string employerAccountId);
    }
}