using System;
using System.Net.Http;
using System.Threading.Tasks;
using DfE.EmployerFavourites.ApplicationServices.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
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
        private readonly AsyncRetryPolicy _retryPolicy;

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
            _retryPolicy = GetRetryPolicy();
        }

        public async Task<string> GetApprenticeshipNameAsync(string apprenticeshipId)
        {
            if (IsStandard(apprenticeshipId))
            {
                var standard = await _retryPolicy.ExecuteAsync(context => _standardApiClient.GetAsync(apprenticeshipId), new Context(nameof(GetProviderNameAsync)));

                return standard.Title;
            }

            var framework = await _retryPolicy.ExecuteAsync(context => _frameworkApiClient.GetAsync(apprenticeshipId), new Context(nameof(GetProviderNameAsync)));

            return framework.Title;
        }

        public async Task<string> GetProviderNameAsync(int ukprn)
        {
            try
            {
                var provider = await _retryPolicy.ExecuteAsync(context => _providerApiClient.GetAsync(ukprn), new Context(nameof(GetProviderNameAsync)));

                return provider.ProviderName;
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

        private AsyncRetryPolicy GetRetryPolicy()
        {
            return Policy
                    .Handle<HttpRequestException>()
                    .WaitAndRetryAsync(new[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(4)
                    }, (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Error calling Fat Api for - {context.OperationKey} Reason: {exception.Message}. Retrying in {timeSpan.Seconds} secs...attempt: {retryCount}");
                    });
        }
    }
}
