using System.Threading.Tasks;
using Refit;

namespace DfE.EmployerFavourites.Web.Infrastructure.FatApiClient
{
    public interface IFatApi
    {
        [Get("/standards/{id}")]
        Task<FatStandard> GetStandardAsync(string id);

        [Get("/frameworks/{id}")]
        Task<FatFramework> GetFrameworkAsync(string id);
    }
}


