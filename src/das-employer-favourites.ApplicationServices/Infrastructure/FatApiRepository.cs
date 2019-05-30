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

        public string GetApprenticeshipName(string apprenticeshipId)
        {
            if (IsStandard(apprenticeshipId))
            {
                return _standardApiClient.Get(apprenticeshipId).Title;
            }
            else
            {
                return _frameworkApiClient.Get(apprenticeshipId).Title;
            }
        }

        public string GetProviderName(string ukprn)
        {
            return _providerApiClient.Get(ukprn)?.ProviderName;
        }

        private bool IsStandard(string apprenticeshipId)
        {
            int standardId;
            return int.TryParse(apprenticeshipId, out standardId);
        }
    }
}
