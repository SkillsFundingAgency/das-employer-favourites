using DfE.EmployerFavourites.ApplicationServices.Domain;
using DfE.EmployerFavourites.ApplicationServices.Infrastructure.Interfaces;
using SFA.DAS.Apprenticeships.Api.Client;

namespace DfE.EmployerFavourites.ApplicationServices.Infrastructure
{
    public class FatApiRepository : IFatRepository
    {
        private readonly IStandardApiClient _standardApiClient;
        private readonly IFrameworkApiClient _frameworkApiClient;

        public FatApiRepository(IStandardApiClient standardApiClient, IFrameworkApiClient frameworkApiClient)
        {
            _standardApiClient = standardApiClient;
            _frameworkApiClient = frameworkApiClient;
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

        private bool IsStandard(string apprenticeshipId)
        {
            int standardId;
            return int.TryParse(apprenticeshipId, out standardId);
        }
    }
}
