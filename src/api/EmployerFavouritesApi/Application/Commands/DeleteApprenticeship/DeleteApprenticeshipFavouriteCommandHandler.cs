using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Api.Domain;
using DfE.EmployerFavourites.Api.Domain.WriteModel;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Api.Types.Exceptions;

namespace DfE.EmployerFavourites.Api.Application.Commands
{
    public class DeleteApprenticeshipFavouriteCommandHandler : IRequestHandler<DeleteApprenticeshipFavouriteCommand, DeleteApprenticeshipFavouriteCommandResponse>
    {
        private readonly IFavouritesWriteRepository _writeRepository;
        private readonly IFavouritesReadRepository _readRepository;
        private readonly ILogger<DeleteApprenticeshipFavouriteCommandHandler> _logger;

        public DeleteApprenticeshipFavouriteCommandHandler(
            IFavouritesWriteRepository writeRepository,
            IFavouritesReadRepository readRepository,
            ILogger<DeleteApprenticeshipFavouriteCommandHandler> logger)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
            _logger = logger;
        }

        public async Task<DeleteApprenticeshipFavouriteCommandResponse> Handle(DeleteApprenticeshipFavouriteCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling {nameof(DeleteApprenticeshipFavouriteCommand)} for {request.EmployerAccountId}, Apprenticeship: {request.ApprenticeshipId}");

            if (String.IsNullOrWhiteSpace(request.ApprenticeshipId))
            {
                throw new ArgumentException($"ApprenticeshipId must be provided for {nameof(DeleteApprenticeshipFavouriteCommand)}");
            }

            if (String.IsNullOrWhiteSpace(request.EmployerAccountId))
            {
                throw new ArgumentException($"EmployerAccountId must be provided for {nameof(DeleteApprenticeshipFavouriteCommand)}");
            }

            var existingFavourites = await _readRepository.GetApprenticeshipFavourites(request.EmployerAccountId);
            var hasExistingRecord = existingFavourites != null && existingFavourites.Count > 0;

            if (hasExistingRecord == false)
            {
                _logger.LogWarning($"No favourites exist for account {request.EmployerAccountId}");
                return new DeleteApprenticeshipFavouriteCommandResponse { CommandResult = DomainUpdateStatus.Failed };
            }

            if (!existingFavourites.Exists(request.ApprenticeshipId))
            {
                _logger.LogWarning($"requested Apprenticeship Id ({request.ApprenticeshipId}) doesnt exist for account {request.EmployerAccountId}");
                return new DeleteApprenticeshipFavouriteCommandResponse { CommandResult = DomainUpdateStatus.Failed };
            }

            var updatedFavourites = existingFavourites.MapToWriteModel();

            updatedFavourites.Remove(request.ApprenticeshipId);

            await _writeRepository.SaveApprenticeshipFavourites(request.EmployerAccountId, updatedFavourites);

            return new DeleteApprenticeshipFavouriteCommandResponse { CommandResult = DomainUpdateStatus.Deleted };
        }
    }
}
