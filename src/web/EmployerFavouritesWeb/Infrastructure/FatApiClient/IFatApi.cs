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

        [Get("/providers/{ukprn}")]
        Task<FatTrainingProvider> GetProviderAsync(string ukprn);

        [Get("/standards/{id}/providers?ukprn={ukprn}&location={location}")]
        Task<FatProviderLocationAddress> GetStandardLocationInformationAsync(string id, string ukprn, string location);

        [Get("/frameworks/{id}/providers?ukprn={ukprn}&location={location}")]
        Task<FatProviderLocationAddress> GetFrameworkLocationInformationAsync(string id, string ukprn, string location);
    }
}


