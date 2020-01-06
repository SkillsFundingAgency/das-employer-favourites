using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Api.Domain;
using DfE.EmployerFavourites.Api.Domain.WriteModel;
using DfE.EmployerFavourites.Api.Infrastructure.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Providers.Api.Client;

namespace DfE.EmployerFavourites.Api.Application.Commands
{
    public class SaveApprenticeshipFavouriteCommandHandler : IRequestHandler<SaveApprenticeshipFavouriteCommand, SaveApprenticeshipFavouriteCommandResponse>
    {
        private readonly IFavouritesWriteRepository _writeRepository;
        private readonly IFavouritesReadRepository _readRepository;
        private readonly IFatRepository _fatRepository;
        private readonly ILogger<SaveApprenticeshipFavouriteCommandHandler> _logger;

        public SaveApprenticeshipFavouriteCommandHandler(
            IFavouritesWriteRepository writeRepository,
            IFavouritesReadRepository readRepository,
            ILogger<SaveApprenticeshipFavouriteCommandHandler> logger, IFatRepository fatRepository)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
            _logger = logger;
            _fatRepository = fatRepository;
        }

        public async Task<SaveApprenticeshipFavouriteCommandResponse> Handle(SaveApprenticeshipFavouriteCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling {nameof(SaveApprenticeshipFavouriteCommand)} for {request.EmployerAccountId}");

            var existingFavourites = await _readRepository.GetApprenticeshipFavourites(request.EmployerAccountId);
            var hasExistingRecord = existingFavourites != null && existingFavourites.Count > 0;

            var providers = request.Favourites.SelectMany(s => s.Providers);

            foreach (var provider in providers)
            {
                provider.Name = await _fatRepository.GetProviderNameAsync(provider.Ukprn);
            }

            await _writeRepository.SaveApprenticeshipFavourites(request.EmployerAccountId, request.Favourites);

            return new SaveApprenticeshipFavouriteCommandResponse { CommandResult = hasExistingRecord ? DomainUpdateStatus.Updated : DomainUpdateStatus.Created };
        }
    }
}
