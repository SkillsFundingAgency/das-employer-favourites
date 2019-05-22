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

            // Get Account
            var accountTask = _accountApi.GetAccount(request.EmployerAccountId);

            // Get favourites for account
            var favouritesTask = _readRepository.GetApprenticeshipFavourites(request.EmployerAccountId);

            await Task.WhenAll(accountTask, favouritesTask);

            // Build view model
            return new ViewEmployerFavouritesResponse
            {
                EmployerAccount = new Domain.ReadModel.EmployerAccount
                {
                    EmployerAccountId = request.EmployerAccountId,
                    Name = accountTask.Result.DasAccountName
                },
                EmployerFavourites = favouritesTask.Result ?? new Domain.ReadModel.ApprenticeshipFavourites()
            };
        }
    }
}