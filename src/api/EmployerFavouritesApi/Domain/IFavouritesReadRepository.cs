using System.Threading.Tasks;
using DfE.EmployerFavourites.Api.Domain.ReadModel;

namespace DfE.EmployerFavourites.Api.Domain
{
    public interface IFavouritesReadRepository
    {
        Task<ApprenticeshipFavourites> GetApprenticeshipFavourites(string employerAccountId);
    }
}