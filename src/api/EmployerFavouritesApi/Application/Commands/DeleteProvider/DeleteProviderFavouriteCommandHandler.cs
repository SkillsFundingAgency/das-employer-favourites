using System;
using System.Threading;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Api.Domain;
using DfE.EmployerFavourites.Api.Domain.WriteModel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DfE.EmployerFavourites.Api.Application.Commands
{
    public class DeleteProviderFavouriteCommandHandler : IRequestHandler<DeleteProviderFavouriteCommand, DeleteProviderFavouriteCommandResponse>
    {
        private readonly IFavouritesWriteRepository _writeRepository;
        private readonly IFavouritesReadRepository _readRepository;
        private readonly ILogger<DeleteProviderFavouriteCommandHandler> _logger;

        public DeleteProviderFavouriteCommandHandler(
            IFavouritesWriteRepository writeRepository,
            IFavouritesReadRepository readRepository,
            ILogger<DeleteProviderFavouriteCommandHandler> logger)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
            _logger = logger;
        }

        public async Task<DeleteProviderFavouriteCommandResponse> Handle(DeleteProviderFavouriteCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling {nameof(DeleteProviderFavouriteCommand)} for {request.EmployerAccountId}, Apprenticeship: {request.ApprenticeshipId} and Ukprn {request.Ukprn}");

            if (String.IsNullOrWhiteSpace(request.ApprenticeshipId))
            {
                throw new ArgumentException($"ApprenticeshipId must be provided for {nameof(DeleteProviderFavouriteCommand)}");
            }

            if (String.IsNullOrWhiteSpace(request.EmployerAccountId))
            {
                throw new ArgumentException($"EmployerAccountId must be provided for {nameof(DeleteProviderFavouriteCommand)}");
            }

            if (request.Ukprn <= 0)
            {
                throw new ArgumentException($"Valid Ukprn must be provided for {nameof(DeleteProviderFavouriteCommand)}");
            }

            var existingFavourites = await _readRepository.GetApprenticeshipFavourites(request.EmployerAccountId);
            var hasExistingRecord = existingFavourites != null && existingFavourites.Count > 0;

            if (hasExistingRecord == false)
            {
                _logger.LogWarning($"No favourites exist for account {request.EmployerAccountId}");
                return new DeleteProviderFavouriteCommandResponse {CommandResult = DomainUpdateStatus.Failed};
            }

            if (!existingFavourites.Exists(request.ApprenticeshipId, request.Ukprn))
            { 
                _logger.LogWarning($"requested Apprenticeship Id ({request.ApprenticeshipId}) or ukprn ({request.Ukprn} )doesnt exist for account {request.EmployerAccountId}");
                return new DeleteProviderFavouriteCommandResponse { CommandResult = DomainUpdateStatus.Failed };
            }

            var updatedFavourites = existingFavourites.MapToWriteModel();


            updatedFavourites.Remove(request.ApprenticeshipId, request.Ukprn);

            await _writeRepository.SaveApprenticeshipFavourites(request.EmployerAccountId, updatedFavourites);
            return new DeleteProviderFavouriteCommandResponse { CommandResult = DomainUpdateStatus.Deleted };


        }
    }
}
