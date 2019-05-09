using System;
using System.Threading;
using System.Threading.Tasks;
using DfE.EmployerFavourites.ApplicationServices.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DfE.EmployerFavourites.ApplicationServices.Queries
{
    public class GetApprenticeshipFavouriteRequestHandler : IRequestHandler<GetApprenticeshipFavouritesRequest,ApprenticeshipFavourites>
    {
        private readonly ILogger<GetApprenticeshipFavouriteRequestHandler> _logger;
        private readonly IFavouritesRepository _repository;

        public GetApprenticeshipFavouriteRequestHandler(
            ILogger<GetApprenticeshipFavouriteRequestHandler> logger,
            IFavouritesRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<ApprenticeshipFavourites> Handle(GetApprenticeshipFavouritesRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Get apprenticeships favourites requested for account id: {request.EmployerAccountID}");

            if (string.IsNullOrWhiteSpace(request.EmployerAccountID))
            {
                throw new ArgumentException("Employer account Id is required but is not provided");
            }

            var favourites = (await _repository.GetApprenticeshipFavourites(request.EmployerAccountID)) ?? new ApprenticeshipFavourites();

            return favourites;
        }
    }
}