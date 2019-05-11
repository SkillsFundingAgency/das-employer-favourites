using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Domain;
using DfE.EmployerFavourites.Infrastructure.Configuration;
using EmployerFavouritesApi.Client.Api;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace DfE.EmployerFavourites.Infrastructure
{
    public class ApiFavouritesRepository : IFavouritesRepository
    {
        private readonly ILogger<ApiFavouritesRepository> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly EmployerFavouritesApiConfig _apiConfig;
        private readonly AdTokenGenerator _tokenGenerator;

        public ApiFavouritesRepository(
            ILogger<ApiFavouritesRepository> logger, 
            IOptions<EmployerFavouritesApiConfig> options,
            AdTokenGenerator tokenGenerator)
        {
            _logger = logger;
            _retryPolicy = GetRetryPolicy();
            _apiConfig = options.Value;
            _tokenGenerator = tokenGenerator;
        }
        public async Task<ApprenticeshipFavourites> GetApprenticeshipFavourites(string employerAccountId)
        {
            try
            {
                var client = await GetClient();
                var favourites = await _retryPolicy.ExecuteAsync(context => client.GetAsync(employerAccountId), new Context(nameof(GetApprenticeshipFavourites)));

                return Map(favourites);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to Get Apprenticeship Favourites from Api for {employerAccountId}", employerAccountId);
                throw;
            }
        }

        private ApprenticeshipFavourites Map(List<EmployerFavouritesApi.Client.Model.ApprenticeshipFavourite> src)
        {
            var dest = new ApprenticeshipFavourites();

            if (src == null || src.Count == 0)
                return dest;

            dest.AddRange(src.Select(x => new ApprenticeshipFavourite { ApprenticeshipId = x.ApprenticeshipId, Ukprns = x.Ukprns }));

            return dest;
        }

        public Task SaveApprenticeshipFavourites(string employerAccountId, ApprenticeshipFavourites apprenticeshipFavourite)
        {
            throw new NotImplementedException();
        }

        private Polly.Retry.AsyncRetryPolicy GetRetryPolicy()
        {
            return Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(new[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(4)
                    }, (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Error calling employer favourites api {context.OperationKey} Reason: {exception.Message}. Retrying in {timeSpan.Seconds} secs...attempt: {retryCount}");
                    });
        }

        private async Task<ApprenticeshipsApi> GetClient()
        {
            string basePath = _apiConfig.ApiBaseUrl;
            string bearerToken = await _tokenGenerator.Generate(_apiConfig.Tenant, _apiConfig.ClientId, _apiConfig.ClientSecret, _apiConfig.IdentifierUri);

            var clientConfig = new EmployerFavouritesApi.Client.Client.Configuration
            {
                BasePath = basePath
            };

            //Add the key to the Authorization header
            clientConfig.ApiKey["Authorization"] = bearerToken;
            // //Add the key prefix (if one is to be used)
            clientConfig.ApiKeyPrefix["Authorization"] = "Bearer";

            return new ApprenticeshipsApi(clientConfig);
        }
    }

}