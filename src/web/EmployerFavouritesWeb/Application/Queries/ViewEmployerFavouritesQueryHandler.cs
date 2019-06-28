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
        private readonly IAccountApiClient _accountApi;

        public ViewEmployerFavouritesQueryHandler(
            ILogger<ViewEmployerFavouritesQueryHandler> logger,
            IFavouritesReadRepository readRepository,
            IAccountApiClient accountApiClient)
        {
            _logger = logger;
            _readRepository = readRepository;
            _accountApi = accountApiClient;
        }
        public async Task<ViewEmployerFavouritesResponse> Handle(ViewEmployerFavouritesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling ViewEmployerFavouritesQuery for {request.EmployerAccountId}");

            // Get favourites for account
            var favourites = await _readRepository.GetApprenticeshipFavourites(request.EmployerAccountId);

            // Build view model
            return new ViewEmployerFavouritesResponse
            {
                EmployerAccount = new Domain.ReadModel.EmployerAccount
                {
                    EmployerAccountId = request.EmployerAccountId
                },
                EmployerFavourites = favourites ?? new Domain.ReadModel.ApprenticeshipFavourites()
            };
        }
    }
}