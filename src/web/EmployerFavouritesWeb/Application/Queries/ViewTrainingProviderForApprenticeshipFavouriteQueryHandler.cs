using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Domain;
using DfE.EmployerFavourites.Domain.ReadModel;
using DfE.EmployerFavourites.Web.Application.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Account.Api.Client;

namespace DfE.EmployerFavourites.Application.Queries
{
    public class ViewTrainingProviderForApprenticeshipFavouriteQueryHandler : IRequestHandler<ViewTrainingProviderForApprenticeshipFavouriteQuery, ViewTrainingProviderForApprenticeshipFavouriteResponse>
    {
        private readonly ILogger<ViewTrainingProviderForApprenticeshipFavouriteQueryHandler> _logger;
        private readonly IFavouritesReadRepository _readRepository;
        private readonly IAccountApiClient _accountApi;

        public ViewTrainingProviderForApprenticeshipFavouriteQueryHandler(
            ILogger<ViewTrainingProviderForApprenticeshipFavouriteQueryHandler> logger,
            IFavouritesReadRepository readRepository,
            IAccountApiClient accountApiClient)
        {
            _logger = logger;
            _readRepository = readRepository;
            _accountApi = accountApiClient;
        }
        public async Task<ViewTrainingProviderForApprenticeshipFavouriteResponse> Handle(ViewTrainingProviderForApprenticeshipFavouriteQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling {nameof(ViewTrainingProviderForApprenticeshipFavouriteQuery)} for {request.EmployerAccountId}");

            // Get Account
            var accountTask = _accountApi.GetAccount(request.EmployerAccountId);

            // Get favourites for account
            var favouritesTask = _readRepository.GetApprenticeshipFavourites(request.EmployerAccountId);

            await Task.WhenAll(accountTask, favouritesTask);

            // Build view model
            return new ViewTrainingProviderForApprenticeshipFavouriteResponse
            {
                Provider = GetProvider(favouritesTask.Result, request.ApprenticeshipId, request.Ukprn)
            };
        }

        private Provider GetProvider(ApprenticeshipFavourites favourites, string apprenticeshipId, int ukprn)
        {
            var provider = (from fav in favourites
                            from prov in fav.Providers
                            where fav.ApprenticeshipId == apprenticeshipId
                            && prov.Ukprn == ukprn
                            select prov).SingleOrDefault();

            if (provider == null)
                throw new EntityNotFoundException($"Cannot find apprenticeship favourite for apprenticeship: {apprenticeshipId}, provider: {ukprn}");

            return provider;
        } 
    }
}