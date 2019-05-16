using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Account.Api.Client;

namespace DfE.EmployerFavourites.Application.Commands
{
    public class SaveApprenticeshipFavouriteCommandHandler : AsyncRequestHandler<SaveApprenticeshipFavouriteCommand>
    {
        private readonly ILogger<SaveApprenticeshipFavouriteCommandHandler> _logger;
        private readonly IFavouritesReadRepository _readRepository;
        private readonly IFavouritesWriteRepository _writeRepository;
        private readonly IAccountApiClient _accountApiClient;

        public SaveApprenticeshipFavouriteCommandHandler(
            ILogger<SaveApprenticeshipFavouriteCommandHandler> logger,
            IFavouritesReadRepository readRepository,
            IFavouritesWriteRepository writeRepository,
            IAccountApiClient accountApiClient)
        {
            _logger = logger;
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _accountApiClient = accountApiClient;
        }

        protected override async Task Handle(SaveApprenticeshipFavouriteCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling SaveApprenticeshipFavouriteCommand for {request.ApprenticeshipId}");

            var employerAccountId = await GetEmployerAccountId(request.UserId);
            var favourites = (await _readRepository.GetApprenticeshipFavourites(employerAccountId)) ?? new Domain.ReadModel.ApprenticeshipFavourites();

            var writeModel = favourites.MapToWriteModel();

            var existing = favourites.SingleOrDefault(x => x.ApprenticeshipId == request.ApprenticeshipId);

            if (existing == null)
            {
                if (request.Ukprn.HasValue)
                    writeModel.Add(new Domain.WriteModel.ApprenticeshipFavourite(request.ApprenticeshipId, request.Ukprn.Value));
                else
                    writeModel.Add(new Domain.WriteModel.ApprenticeshipFavourite(request.ApprenticeshipId));
            }
            else
            {
                if (!request.Ukprn.HasValue)
                    return;
                else if (existing.Ukprns.Contains(request.Ukprn.Value))
                    return;
                else    
                    existing.Ukprns.Add(request.Ukprn.Value);
            }

            await _writeRepository.SaveApprenticeshipFavourites(employerAccountId, writeModel);
        }

        private async Task<string> GetEmployerAccountId(string userId)
        {
            try
            {
                var accounts = await _accountApiClient.GetUserAccounts(userId);
                
                return accounts.First().HashedAccountId;

                    // Currenly the API isn't passing through the correct details
                    // of the registered date
                    // We may be able to rely on the order from the api. They do 
                    // look like they're returned in the order they are created.
                    // .OrderBy(x => x.DateRegistered)
                    // .ThenBy(x => x.HashedAccountId)
                    // .First().HashedAccountId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve account information for user Id: {userId}");
                throw;
            }
        }
    }
}