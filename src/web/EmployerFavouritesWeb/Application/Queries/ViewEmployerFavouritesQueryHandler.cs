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
        private readonly IFavouritesRepository _repository;
        private readonly IAccountApiClient _accountApi;

        public ViewEmployerFavouritesQueryHandler(
            ILogger<ViewEmployerFavouritesQueryHandler> logger,
            IFavouritesRepository repository, 
            IAccountApiClient accountApiClient)
        {
            _logger = logger;
            _repository = repository;
            _accountApi = accountApiClient;
        }
        public async Task<ViewEmployerFavouritesResponse> Handle(ViewEmployerFavouritesQuery request, CancellationToken cancellationToken)
        {
            // Get Account
            var accountTask = _accountApi.GetAccount(request.EmployerAccountId);

            // Get favourites for account
            var favouritesTask = _repository.GetApprenticeshipFavourites(request.EmployerAccountId);

            await Task.WhenAll(accountTask, favouritesTask);

            // Build view model
            return new ViewEmployerFavouritesResponse
            {
                EmployerAccount = new EmployerAccount
                {
                    EmployerAccountId = request.EmployerAccountId,
                    Name = accountTask.Result.DasAccountName
                },
                EmployerFavourites = favouritesTask.Result ?? new ApprenticeshipFavourites()
            };
        }
    }
}