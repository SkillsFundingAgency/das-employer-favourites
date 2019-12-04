using System.Threading;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DfE.EmployerFavourites.Application.Commands
{
    public class DeleteApprenticeshipFavouriteCommandHandler : IRequestHandler<DeleteApprenticeshipFavouriteCommand>
    {
        private readonly ILogger<DeleteApprenticeshipFavouriteCommandHandler> _logger;
        private readonly IFavouritesWriteRepository _writeRepository;

        public DeleteApprenticeshipFavouriteCommandHandler(
            ILogger<DeleteApprenticeshipFavouriteCommandHandler> logger,
            IFavouritesReadRepository readRepository,
            IFavouritesWriteRepository writeRepository)
        {
            _logger = logger;
            _writeRepository = writeRepository;
        }

        public async Task<Unit> Handle(DeleteApprenticeshipFavouriteCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling DeleteApprenticeshipFavouriteCommandHandler for Employer Account ID: {employerAccountId}, apprenticeship Id: {apprenticeshipId}", request.EmployerAccountId, request.ApprenticeshipId);

            await _writeRepository.DeleteApprenticeshipFavourites(request.EmployerAccountId, request.ApprenticeshipId);
                _logger.LogDebug("Deleted favourite for Apprenticeship Id: {apprenticeshipId}  for Employer Id:{employerId}", request.ApprenticeshipId, request.EmployerAccountId);
           
            return Unit.Value;
        }
    }
}