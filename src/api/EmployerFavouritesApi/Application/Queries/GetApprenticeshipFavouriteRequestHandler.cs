using DfE.EmployerFavourites.Api.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Api.Domain.ReadModel;

namespace DfE.EmployerFavourites.Api.Application.Queries
{
    public class GetApprenticeshipFavouriteRequestHandler : IRequestHandler<GetApprenticeshipFavouritesRequest, ApprenticeshipFavourites>
    {
        private readonly ILogger<GetApprenticeshipFavouriteRequestHandler> _logger;
        private readonly IFavouritesReadRepository _repository;

        public GetApprenticeshipFavouriteRequestHandler(
            ILogger<GetApprenticeshipFavouriteRequestHandler> logger,
            IFavouritesReadRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<ApprenticeshipFavourites> Handle(GetApprenticeshipFavouritesRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling GetApprenticeshipFavouritesRequest for {request.EmployerAccountID}");

            if (string.IsNullOrWhiteSpace(request.EmployerAccountID))
            {
                throw new ArgumentException("Employer account Id is required but is not provided");
            }

            var favourites = (await _repository.GetApprenticeshipFavourites(request.EmployerAccountID)) ?? new ApprenticeshipFavourites();

            return favourites;
        }
    }
}