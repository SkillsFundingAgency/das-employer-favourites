using System.Threading.Tasks;
using DfE.EmployerFavourites.ApplicationServices.Domain.WriteModel;

namespace DfE.EmployerFavourites.ApplicationServices.Domain
{
    public interface IFavouritesWriteRepository
    {
        Task SaveApprenticeshipFavourites(string employerAccountId, ApprenticeshipFavourites apprenticeshipFavourite);
    }
}