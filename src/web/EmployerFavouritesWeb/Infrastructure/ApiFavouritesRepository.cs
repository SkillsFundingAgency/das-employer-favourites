using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Domain;
using DfE.EmployerFavourites.Domain.WriteModel;
using DfE.EmployerFavourites.Infrastructure.Configuration;
using DfE.EmployerFavourites.Web.Infrastructure.FavouritesApiClient;
using EmployerFavouritesApi.Client.Api;
using EmployerFavouritesApi.Client.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RestSharp;

namespace DfE.EmployerFavourites.Infrastructure
{
    public class ApiFavouritesRepository : IFavouritesReadRepository, IFavouritesWriteRepository
    {
        private readonly ILogger<ApiFavouritesRepository> _logger;
        private readonly IFavouritesApi _favouritesApi;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly EmployerFavouritesApiConfig _apiConfig;

        public ApiFavouritesRepository(
            ILogger<ApiFavouritesRepository> logger, 
            IOptions<EmployerFavouritesApiConfig> options,
            IFavouritesApi favouritesApi)
        {
            _logger = logger;
            _favouritesApi = favouritesApi;
            _retryPolicy = GetRetryPolicy();
            _apiConfig = options.Value;
        }


        public async Task SaveApprenticeshipFavourites(string employerAccountId, ApprenticeshipFavourites apprenticeshipFavourites)
        {
            try
            {
                await _retryPolicy.ExecuteAsync(context => _favouritesApi.PutAsync(employerAccountId, apprenticeshipFavourites), new Context(nameof(GetApprenticeshipFavourites)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to Save Apprenticeship Favourites to Api for Account: {employerAccountId}", employerAccountId);
                throw;
            }
        }

        public virtual async Task<Domain.ReadModel.ApprenticeshipFavourites> GetApprenticeshipFavourites(string employerAccountId)
        {
            try
            {
                var favourites = await _retryPolicy.ExecuteAsync(context => _favouritesApi.GetAsync(employerAccountId), new Context(nameof(GetApprenticeshipFavourites)));

                return favourites;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to Get Apprenticeship Favourites from Api for {employerAccountId}", employerAccountId);
                throw;
            }
        }

        private AsyncRetryPolicy GetRetryPolicy()
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
    }
}