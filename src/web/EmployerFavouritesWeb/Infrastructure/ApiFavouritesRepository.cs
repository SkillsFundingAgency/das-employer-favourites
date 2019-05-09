using System;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Web.Configuration;
using DfE.EmployerFavourites.Web.Domain;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace DfE.EmployerFavourites.Web.Infrastructure
{
    public class ApiFavouritesRepository : IFavouritesRepository
    {
        private readonly ILogger<ApiFavouritesRepository> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;

        public ApiFavouritesRepository(ILogger<ApiFavouritesRepository> logger)
        {
            
            _logger = logger;
            _retryPolicy = GetRetryPolicy();
        }
        public async Task<ApprenticeshipFavourites> GetApprenticeshipFavourites(string employerAccountId)
        {
            try
            {
                await _retryPolicy.ExecuteAsync(context => Task.CompletedTask, new Context(nameof(GetApprenticeshipFavourites)));
                
                return null;
            }
            catch (Exception ex) // TODO: LWA Catch more specific exceptions?
            {
                _logger.LogError(ex, "Unable to Get Apprenticeship Favourites for {employerAccountId}", employerAccountId);
                throw;
            }
        }

        public async Task SaveApprenticeshipFavourites(string employerAccountId, ApprenticeshipFavourites apprenticeshipFavourite)
        {
            if (apprenticeshipFavourite == null)
            {
                throw new ArgumentNullException("apprenticeshipFavourite");
            }

            try
            {
                await _retryPolicy.ExecuteAsync(context => Task.CompletedTask, new Context(nameof(GetApprenticeshipFavourites)));
            }
            catch (Exception ex) // TODO: LWA Catch more specific exceptions?
            {
                 _logger.LogError(ex, "Unable to Save Apprenticeship Favourites for {employerAccountId}", employerAccountId);
                throw;
            }
        }

        private Polly.Retry.AsyncRetryPolicy GetRetryPolicy()
        {
            return Policy
                    .Handle<StorageException>() // TODO: LWA Determine what exceptions to retry on.
                    .WaitAndRetryAsync(new[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(4)
                    }, (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Error executing Table Storage command  {context.OperationKey} Reason: {exception.Message}. Retrying in {timeSpan.Seconds} secs...attempt: {retryCount}");
                    });
        }
    }
}