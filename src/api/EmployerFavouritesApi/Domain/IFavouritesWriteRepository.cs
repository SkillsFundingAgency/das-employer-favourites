using System.Threading.Tasks;
using DfE.EmployerFavourites.Api.Domain.WriteModel;

namespace DfE.EmployerFavourites.Api.Domain
{
    public interface IFavouritesWriteRepository
    {
        Task SaveApprenticeshipFavourites(string employerAccountId, ApprenticeshipFavourites apprenticeshipFavourite);
    }
}