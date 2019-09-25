using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Domain;
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

        public ViewTrainingProviderForApprenticeshipFavouriteQueryHandler(
            ILogger<ViewTrainingProviderForApprenticeshipFavouriteQueryHandler> logger,
            IFavouritesReadRepository readRepository)
        {
            _logger = logger;
            _readRepository = readRepository;
        }

        public async Task<ViewTrainingProviderForApprenticeshipFavouriteResponse> Handle(ViewTrainingProviderForApprenticeshipFavouriteQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling {nameof(ViewTrainingProviderForApprenticeshipFavouriteQuery)} for {request.EmployerAccountId}");

            // Get favourites for account
            var favourites = await _readRepository.GetApprenticeshipFavourites(request.EmployerAccountId);


            var apprenticeship = favourites.SingleOrDefault(x => x.ApprenticeshipId == request.ApprenticeshipId);

            if (apprenticeship == null)
                throw new EntityNotFoundException($"Cannot find apprenticeship favourite for employer account: {{request.EmployerAccountId}} and apprenticeshipId: {{request.ApprenticeshipId}}");


            var provider = apprenticeship.Providers.SingleOrDefault(w => w.Ukprn == request.Ukprn);

            if (provider == null)
                throw new EntityNotFoundException($"Cannot find Training Provider for employer account: {{request.EmployerAccountId}}, apprenticeshipId: {{request.ApprenticeshipId}} and UKPRN: {{request.Ukprn}}");

            // Build view model
            return new ViewTrainingProviderForApprenticeshipFavouriteResponse
            {
                Provider = provider
            };
        }
    }
}