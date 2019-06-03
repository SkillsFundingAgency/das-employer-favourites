using System.Threading.Tasks;
using DfE.EmployerFavourites.ApplicationServices.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Types.Exceptions;
using SFA.DAS.Providers.Api.Client;

namespace DfE.EmployerFavourites.ApplicationServices.Infrastructure
{
    public class FatApiRepository : IFatRepository
    {
        private readonly IStandardApiClient _standardApiClient;
        private readonly IFrameworkApiClient _frameworkApiClient;
        private readonly IProviderApiClient _providerApiClient;
        private readonly ILogger<FatApiRepository> _logger;

        public FatApiRepository(
            IStandardApiClient standardApiClient,
            IFrameworkApiClient frameworkApiClient,
            IProviderApiClient providerApiClient,
            ILogger<FatApiRepository> logger)
        {
            _standardApiClient = standardApiClient;
            _frameworkApiClient = frameworkApiClient;
            _providerApiClient = providerApiClient;
            _logger = logger;
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
            try
            {
                return (await _providerApiClient.GetAsync(ukprn)).ProviderName;
            }
            catch(EntityNotFoundException ex)
            {
                _logger.LogError(ex, $"Fat Api didn't find a provider for {ukprn}");

                return $"Unknown Provider ({ukprn})";
            }
        }

        private bool IsStandard(string apprenticeshipId)
        {
            return int.TryParse(apprenticeshipId, out int _);
        }
    }
}
