using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Api.Domain;
using DfE.EmployerFavourites.Api.Domain.WriteModel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DfE.EmployerFavourites.Api.Application.Commands
{
    public class SaveApprenticeshipFavouriteCommandHandler : IRequestHandler<SaveApprenticeshipFavouriteCommand, SaveApprenticeshipFavouriteCommandResponse>
    {
        private readonly IFavouritesWriteRepository _writeRepository;
        private readonly IFavouritesReadRepository _readRepository;
        private readonly ILogger<SaveApprenticeshipFavouriteCommandHandler> _logger;

        public SaveApprenticeshipFavouriteCommandHandler(
            IFavouritesWriteRepository writeRepository,
            IFavouritesReadRepository readRepository,
            ILogger<SaveApprenticeshipFavouriteCommandHandler> logger)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
            _logger = logger;
        }

        public async Task<SaveApprenticeshipFavouriteCommandResponse> Handle(SaveApprenticeshipFavouriteCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling {nameof(SaveApprenticeshipFavouriteCommand)} for {request.EmployerAccountId}");

            var existingFavourites = await _readRepository.GetApprenticeshipFavourites(request.EmployerAccountId);
            var hasExistingRecord = existingFavourites != null && existingFavourites.Count > 0;

            foreach (var provider in request.Favourites.SelectMany(w => w.Providers))
            {
                
            }


            await _writeRepository.SaveApprenticeshipFavourites(request.EmployerAccountId, request.Favourites);

            return new SaveApprenticeshipFavouriteCommandResponse { CommandResult = hasExistingRecord ? DomainUpdateStatus.Updated : DomainUpdateStatus.Created };
        }
    }
}
