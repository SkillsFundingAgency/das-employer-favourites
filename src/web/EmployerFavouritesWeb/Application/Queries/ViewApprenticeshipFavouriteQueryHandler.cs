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
    public class ViewApprenticeshipFavouriteQueryHandler : IRequestHandler<ViewApprenticeshipFavouriteQuery, ViewApprenticeshipFavouriteResponse>
    {
        private readonly ILogger<ViewTrainingProviderForApprenticeshipFavouriteQueryHandler> _logger;
        private readonly IFavouritesReadRepository _readRepository;

        public ViewApprenticeshipFavouriteQueryHandler(
            ILogger<ViewTrainingProviderForApprenticeshipFavouriteQueryHandler> logger,
            IFavouritesReadRepository readRepository)
        {
            _logger = logger;
            _readRepository = readRepository;
        }

        public async Task<ViewApprenticeshipFavouriteResponse> Handle(ViewApprenticeshipFavouriteQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling {nameof(ViewApprenticeshipFavouriteQuery)} for {request.EmployerAccountId}");

            // Get favourites for account
            var favourites = await _readRepository.GetApprenticeshipFavourites(request.EmployerAccountId);

            var favourite = favourites.SingleOrDefault(x => x.ApprenticeshipId == request.ApprenticeshipId);

            if (favourite == null)
                throw new EntityNotFoundException($"Cannot find apprenticeship favourite for employer account: {{request.EmployerAccountId}} and apprenticeshipId: {request.ApprenticeshipId}");

            // Build view model
            return new ViewApprenticeshipFavouriteResponse
            {
                Favourite = favourite
            };
        }
    }
}