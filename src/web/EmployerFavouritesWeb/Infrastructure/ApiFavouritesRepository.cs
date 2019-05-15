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
using RestSharp;

namespace DfE.EmployerFavourites.Infrastructure
{
    public class ApiFavouritesRepository : IFavouritesReadRepository
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

        public virtual async Task<Domain.ReadModel.ApprenticeshipFavourites> GetApprenticeshipFavourites(string employerAccountId)
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

        private Domain.ReadModel.ApprenticeshipFavourites Map(List<EmployerFavouritesApi.Client.Model.ApprenticeshipFavourite> src)
        {
            var dest = new Domain.ReadModel.ApprenticeshipFavourites();

            if (src == null || src.Count == 0)
                return dest;

            dest.AddRange(src.Select(x => new Domain.ReadModel.ApprenticeshipFavourite 
                { 
                    ApprenticeshipId = x.ApprenticeshipId, 
                    Title = x.Title,
                    Ukprns = x.Ukprns 
                }));

            return dest;
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
            string bearerToken = String.Empty;

            if (!_apiConfig.HasEmptyProperties())
            {
                bearerToken = await _tokenGenerator.Generate(_apiConfig.Tenant, _apiConfig.ClientId, _apiConfig.ClientSecret, _apiConfig.IdentifierUri);
            }

            var clientConfig = new EmployerFavouritesApi.Client.Client.Configuration
            {
                BasePath = basePath
            };

            clientConfig.AccessToken = bearerToken;

            return new ApprenticeshipsApi(clientConfig);
        }
    }

}