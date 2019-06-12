using System.Threading.Tasks;

namespace DfE.EmployerFavourites.ApplicationServices.Infrastructure.Interfaces
{
    public interface IFatRepository
    {
        Task<string> GetApprenticeshipNameAsync(string apprenticeshipId);
        Task<string> GetProviderNameAsync(int ukprn);
    }
}