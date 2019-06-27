using System.Threading.Tasks;

namespace DfE.EmployerFavourites.Api.Domain
{
    public interface IEmployerAccountRepository
    {
        Task<string> GetEmployerAccountId(string userId);
    }
}