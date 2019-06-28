using System;
using System.Linq;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Domain;
using DfE.EmployerFavourites.Web.Infrastructure.FatApiClient;
using DfE.EmployerFavourites.Web.Infrastructure.FavouritesApiClient;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using ReadModel = DfE.EmployerFavourites.Domain.ReadModel;
using WriteModel = DfE.EmployerFavourites.Domain.WriteModel;

namespace DfE.EmployerFavourites.Infrastructure
{
    public class FavouritesRepository : IFavouritesReadRepository, IFavouritesWriteRepository
    {
        private readonly ILogger<FavouritesRepository> _logger;
        private readonly IFavouritesApi _favouritesApi;
        private readonly IFatApi _fatApi;
        private readonly AsyncRetryPolicy _retryPolicy;

        public FavouritesRepository(
            ILogger<FavouritesRepository> logger, 
            IFavouritesApi favouritesApi,
            IFatApi fatApi)
        {
            _logger = logger;
            _favouritesApi = favouritesApi;
            _fatApi = fatApi;
            _retryPolicy = GetRetryPolicy();
        }

        public async Task SaveApprenticeshipFavourites(string employerAccountId, WriteModel.ApprenticeshipFavourites apprenticeshipFavourites)
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

        public virtual async Task<ReadModel.ApprenticeshipFavourites> GetApprenticeshipFavourites(string employerAccountId)
        {
            try
            {
                var favourites = await _retryPolicy.ExecuteAsync(context => _favouritesApi.GetAsync(employerAccountId), new Context(nameof(GetApprenticeshipFavourites)));

                await EnrichFavourites(favourites);

                return favourites;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to Get Apprenticeship Favourites from Api for {employerAccountId}", employerAccountId);
                throw;
            }
        }

        private async Task EnrichFavourites(ReadModel.ApprenticeshipFavourites favourites)
        {
            if (favourites != null)
            {
                var tasks = favourites.Select(UpdateApprenticeship).ToList();
                await Task.WhenAll(tasks);
            }
        }

        private async Task UpdateApprenticeship(ReadModel.ApprenticeshipFavourite favourite)
        {
            if (favourite.IsFramework)
            {
                var framework = await _fatApi.GetFrameworkAsync(favourite.ApprenticeshipId);
                favourite.Level = framework.Level;
                favourite.TypicalLength = framework.Duration;
                favourite.ExpiryDate = framework.ExpiryDate;
            }
            else
            {
                var standard = await _fatApi.GetStandardAsync(favourite.ApprenticeshipId);
                favourite.Level = standard.Level;
                favourite.TypicalLength = standard.Duration;
                favourite.ExpiryDate = null;
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