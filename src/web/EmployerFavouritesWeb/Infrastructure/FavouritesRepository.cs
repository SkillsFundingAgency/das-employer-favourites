using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Domain;
using DfE.EmployerFavourites.Domain.ReadModel;
using DfE.EmployerFavourites.Web.Infrastructure.FatApiClient;
using DfE.EmployerFavourites.Web.Infrastructure.FavouritesApiClient;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Refit;
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
                await _retryPolicy.ExecuteAsync(context => _favouritesApi.PutAsync(employerAccountId, apprenticeshipFavourites), new Context(nameof(SaveApprenticeshipFavourites)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to Save Apprenticeship Favourites to Api for Account: {employerAccountId}", employerAccountId);
                throw;
            }
        }

        public async Task DeleteApprenticeshipFavourites(string employerAccountId, string apprenticeshipId)
        {
            try
            {
                await _retryPolicy.ExecuteAsync(context => _favouritesApi.DeleteAsync(employerAccountId, apprenticeshipId), new Context(nameof(DeleteApprenticeshipFavourites)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to Delete Apprenticeship Favourites to Api for Account: {employerAccountId} and apprenticeshipId:", employerAccountId, apprenticeshipId);
                throw;
            }
        }

        public async Task DeleteApprenticeshipProviderFavourites(string employerAccountId, string apprenticeshipId, int ukprn)
        {
            try
            {
                await _retryPolicy.ExecuteAsync(context => _favouritesApi.DeleteAsync(employerAccountId, apprenticeshipId, ukprn), new Context(nameof(DeleteApprenticeshipProviderFavourites)));
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
                var apprenticeshipUpdateTasks = favourites.Select(UpdateApprenticeship).ToList();

                var providers = favourites.SelectMany(x => x.Providers)?.Select(y => y.Ukprn).Distinct();

                List<Task<FatTrainingProvider>> fatProviderTasks = null;

                List<Task<FatProviderLocationAddress>> fatGetLocationTasks = null;

                if (providers != null)
                {
                    fatProviderTasks = providers.Select(x => GetProviderDetails(x.ToString())).ToList();
                }

                var providerLocationsList = favourites.Where(w => w.Providers != null).SelectMany(a => a.Providers.Where(w => w.LocationIds != null), (a, p) => new { apprenticeshipId = a.ApprenticeshipId, isFramework = a.IsFramework, ukprn = p.Ukprn, locations = p.LocationIds });

                fatGetLocationTasks = providerLocationsList.SelectMany(l => l.locations.Select(s => GetProviderLocations(l.apprenticeshipId, l.isFramework, l.ukprn, s))).Select(x => x).ToList();

                await Task.WhenAll(apprenticeshipUpdateTasks.Concat(fatProviderTasks).Concat(fatGetLocationTasks));

                UpdateTrainingProviders(favourites, fatProviderTasks.Where(w => w.Result != null).Select(x => x.Result));

                UpdateLocations(favourites, fatGetLocationTasks.Where(w => w.Result != null).Select(y => y.Result).GroupBy(x => x.Location.LocationId).Select(g => g.First()));
            }
        }


        private async Task<FatTrainingProvider> GetProviderDetails(string ukprn)
        {
            try
            {
                return await _fatApi.GetProviderAsync(ukprn);
            }
            catch (ApiException e)
            {
                _logger.LogWarning(e, $"The requested ukprn: {ukprn}, doesnt exist, this is probably due to a favourite item no longer being available");
            }

            return null;
        }

        private void UpdateLocations(ReadModel.ApprenticeshipFavourites favourites, IEnumerable<FatProviderLocationAddress> locationData)
        {
            foreach (var provider in favourites.SelectMany(s => s.Providers).Where(w => w != null && w.LocationIds != null))
            {
                provider.Locations = locationData.Where(w => provider.LocationIds.Contains(w.Location.LocationId)).Select(UpdateLocation).ToList();
            }
        }

        private Location UpdateLocation(FatProviderLocationAddress fatLocation)
        {
            var location = new Location();

            location.Name = fatLocation.Location.LocationName;
            location.Address1 = fatLocation.Location.Address.Address1;
            location.Address2 = fatLocation.Location.Address.Address2;
            location.Town = fatLocation.Location.Address.Town;
            location.PostCode = fatLocation.Location.Address.PostCode;
            location.LocationId = fatLocation.Location.LocationId;

            return location;
        }

        private async Task<FatProviderLocationAddress> GetProviderLocations(string apprenticeshipId, bool isFramework, int ukprn, int loc)
        {
            try
            {
                if (isFramework)
                {
                    return await _fatApi.GetFrameworkLocationInformationAsync(apprenticeshipId, ukprn.ToString(), loc.ToString());
                }

                return await _fatApi.GetStandardLocationInformationAsync(apprenticeshipId, ukprn.ToString(), loc.ToString());
            }
            catch (ApiException e)
            {
                _logger.LogWarning(e, $"The requested ukprn: {ukprn}, doesnt exist, this is probably due to a favourite item no longer being available");
                return null;
            }
        }

        private void UpdateTrainingProviders(ReadModel.ApprenticeshipFavourites favourites, IEnumerable<FatTrainingProvider> providerData)
        {
            foreach (var item in providerData)
            {
                var matchingProviders = favourites.SelectMany(x => x.Providers).Where(y => y.Ukprn == item.Ukprn);

                List<Location> locations = new List<Location>();

                foreach (var provider in matchingProviders)
                {
                    provider.Name = item.ProviderName;
                    provider.Phone = item.Phone;
                    provider.Email = item.Email;
                    provider.Website = item.Website;
                    provider.EmployerSatisfaction = item.EmployerSatisfaction;
                    provider.LearnerSatisfaction = item.LearnerSatisfaction;
                    provider.Address = item.Addresses.Find(x => x.ContactType == "PRIMARY");
                    provider.Active = true;

                }
            }
        }

        private async Task UpdateApprenticeship(ReadModel.ApprenticeshipFavourite favourite)
        {
            if (favourite.IsFramework)
            {

                var framework = await _fatApi.GetFrameworkAsync(favourite.ApprenticeshipId);
                favourite.Title = framework.Title;
                favourite.Level = framework.Level;
                favourite.TypicalLength = framework.Duration;
                favourite.ExpiryDate = framework.ExpiryDate;
                favourite.Active = framework.IsActiveFramework;
            }
            else
            {
                var standard = await _fatApi.GetStandardAsync(favourite.ApprenticeshipId);
                favourite.Title = standard.Title;
                favourite.Level = standard.Level;
                favourite.TypicalLength = standard.Duration;
                favourite.ExpiryDate = null;
                favourite.Active = standard.IsActiveStandard;
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