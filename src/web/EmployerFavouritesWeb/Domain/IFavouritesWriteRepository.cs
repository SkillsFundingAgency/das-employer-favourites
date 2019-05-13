using System.Threading.Tasks;

namespace DfE.EmployerFavourites.Domain
{
    public interface IFavouritesWriteRepository
    {
        Task SaveApprenticeshipFavourites(string employerAccountId, WriteModel.ApprenticeshipFavourites apprenticeshipFavourite);
    }
}