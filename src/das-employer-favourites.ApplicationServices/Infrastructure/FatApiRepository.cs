using System.Threading.Tasks;
using DfE.EmployerFavourites.ApplicationServices.Infrastructure.Interfaces;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Providers.Api.Client;

namespace DfE.EmployerFavourites.ApplicationServices.Infrastructure
{
    public class FatApiRepository : IFatRepository
    {
        private readonly IStandardApiClient _standardApiClient;
        private readonly IFrameworkApiClient _frameworkApiClient;
        private readonly IProviderApiClient _providerApiClient;

        public FatApiRepository(
            IStandardApiClient standardApiClient, 
            IFrameworkApiClient frameworkApiClient,
            IProviderApiClient providerApiClient)
        {
            _standardApiClient = standardApiClient;
            _frameworkApiClient = frameworkApiClient;
            _providerApiClient = providerApiClient;
        }

        public async Task<string> GetApprenticeshipNameAsync(string apprenticeshipId)
        {
            if (IsStandard(apprenticeshipId))
            {
                return (await _standardApiClient.GetAsync(apprenticeshipId)).Title;
            }

            return (await _frameworkApiClient.GetAsync(apprenticeshipId)).Title;
        }

        public async Task<string> GetProviderNameAsync(int ukprn)
        {
            return (await _providerApiClient.GetAsync(ukprn)).ProviderName;
        }

        private bool IsStandard(string apprenticeshipId)
        {
            return int.TryParse(apprenticeshipId, out int _);
        }
    }
}
