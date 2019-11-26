using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Sfa.Das.Sas.Shared.Basket.Interfaces;
using SFA.DAS.EAS.Account.Api.Client;

namespace DfE.EmployerFavourites.Application.Commands
{
    public class SaveApprenticeshipFavouriteBasketCommandHandler : IRequestHandler<SaveApprenticeshipFavouriteBasketCommand, string>
    {
        private readonly ILogger<SaveApprenticeshipFavouriteBasketCommandHandler> _logger;
        private readonly IFavouritesReadRepository _readRepository;
        private readonly IFavouritesWriteRepository _writeRepository;
        private readonly IAccountApiClient _accountApiClient;
        private readonly IApprenticeshipFavouritesBasketStore _basketStore;

        public SaveApprenticeshipFavouriteBasketCommandHandler(
            ILogger<SaveApprenticeshipFavouriteBasketCommandHandler> logger,
            IFavouritesReadRepository readRepository,
            IFavouritesWriteRepository writeRepository,
            IAccountApiClient accountApiClient,
            IApprenticeshipFavouritesBasketStore basketStore)
        {
            _logger = logger;
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _accountApiClient = accountApiClient;
            _basketStore = basketStore;
        }

        public async Task<string> Handle(SaveApprenticeshipFavouriteBasketCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling SaveApprenticeshipFavouriteCommand for basket: {basketId}", request.BasketId);

            var basketContentTask = _basketStore.GetAsync(request.BasketId);
            var employerAccountId = await GetEmployerAccountId(request.UserId);
            var favouritesTask = _readRepository.GetApprenticeshipFavourites(employerAccountId);

            await Task.WhenAll(basketContentTask, favouritesTask);

            var favourites = favouritesTask.Result ?? new Domain.ReadModel.ApprenticeshipFavourites();
            var basketContent = basketContentTask.Result;

            var writeModel = favourites.MapToWriteModel();

            if (basketContent == null || !basketContent.Any())
                return employerAccountId;

            bool changesMade = false;

            foreach(var item in basketContent)
            {
                changesMade |= writeModel.Update(item.ApprenticeshipId, item.Ukprns);
            }

            if (changesMade)
            {
                await _writeRepository.SaveApprenticeshipFavourites(employerAccountId, writeModel);
                _logger.LogDebug("Updated basket: {basketId}", request.BasketId);
            }
            else
            {
                _logger.LogDebug("No changes required for basket: {basketId}", request.BasketId);
            }

            return employerAccountId;
        }

        private async Task<string> GetEmployerAccountId(string userId)
        {
            try
            {
                var accounts = await _accountApiClient.GetUserAccounts(userId);
                
                return accounts
                    .OrderBy(x => x.AccountId) // AccountId is ascendingly sequential
                    .First().HashedAccountId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve account information for user Id: {userId}");
                throw;
            }
        }
    }
}