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
            _logger.LogInformation($"Handling SaveApprenticeshipFavouriteCommand for {request.ApprenticeshipId}");

            var employerAccountId = await GetEmployerAccountId(request.UserId);
            var favourites = (await _readRepository.GetApprenticeshipFavourites(employerAccountId)) ?? new Domain.ReadModel.ApprenticeshipFavourites();
            var writeModel = favourites.MapToWriteModel();


            // TODO: LWA don't await the individual tasks
            var basketContent = await _basketStore.GetAsync(request.BasketId);

            bool contentHaveChanged = false;

            if (basketContent == null || basketContent.Count == 0)
                return employerAccountId;

            foreach(var item in basketContent)
            {
                var existing = writeModel.SingleOrDefault(x => x.ApprenticeshipId == item.ApprenticeshipId);

                if (existing == null)
                {
                    if (item.Ukprns == null || item.Ukprns.Count == 0)
                    {
                        writeModel.Add(new Domain.WriteModel.ApprenticeshipFavourite(item.ApprenticeshipId));
                        contentHaveChanged = true;
                    }
                    else
                    {
                        writeModel.Add(new Domain.WriteModel.ApprenticeshipFavourite(item.ApprenticeshipId, item.Ukprns));
                        contentHaveChanged = true;
                    }
                }
                else
                {
                    if (item.Ukprns != null || item.Ukprns.Count > 0)
                    {
                        foreach(var ukprn in item.Ukprns)
                        {
                            if (!existing.Ukprns.Contains(ukprn))
                            {
                                existing.Ukprns.Add(ukprn);

                                contentHaveChanged = true;
                            }
                        }
                    }
                }
            }

            if (contentHaveChanged)
                await _writeRepository.SaveApprenticeshipFavourites(employerAccountId, writeModel);

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