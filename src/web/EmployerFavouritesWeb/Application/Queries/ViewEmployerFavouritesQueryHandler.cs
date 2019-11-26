using System.Threading;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Account.Api.Client;

namespace DfE.EmployerFavourites.Application.Queries
{
    public class ViewEmployerFavouritesQueryHandler : IRequestHandler<ViewEmployerFavouritesQuery, ViewEmployerFavouritesResponse>
    {
        private readonly ILogger<ViewEmployerFavouritesQueryHandler> _logger;
        private readonly IFavouritesReadRepository _readRepository;
        private readonly IMediator _mediator;

        public ViewEmployerFavouritesQueryHandler(
            ILogger<ViewEmployerFavouritesQueryHandler> logger,
            IFavouritesReadRepository readRepository,
            IMediator mediator)
        {
            _logger = logger;
            _readRepository = readRepository;
            _mediator = mediator;
        }
        public async Task<ViewEmployerFavouritesResponse> Handle(ViewEmployerFavouritesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling ViewEmployerFavouritesQuery for {request.EmployerAccountId}");

            // Get favourites for account
            var favourites = await _readRepository.GetApprenticeshipFavourites(request.EmployerAccountId);

            var employerHasLegalEntityResponse = await _mediator.Send(new EmployerHasLegalEntityQuery
            {
                EmployerAccountId = request.EmployerAccountId
            });
            
            // Build view model
            return new ViewEmployerFavouritesResponse
            {
                EmployerAccount = new Domain.ReadModel.EmployerAccount
                {
                    EmployerAccountId = request.EmployerAccountId,
                    HasLegalEntities = employerHasLegalEntityResponse
                },
                EmployerFavourites = favourites ?? new Domain.ReadModel.ApprenticeshipFavourites()
            };
        }
    }
}