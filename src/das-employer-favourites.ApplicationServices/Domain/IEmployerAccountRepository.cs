using System.Threading.Tasks;

namespace DfE.EmployerFavourites.ApplicationServices.Domain
{
    public interface IEmployerAccountRepository
    {
        Task<string> GetEmployerAccountId(string userId);
    }
}