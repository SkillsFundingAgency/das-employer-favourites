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
    public class DeleteApprenticeshipProviderFavouriteCommandHandler : IRequestHandler<DeleteApprenticeshipProviderFavouriteCommand>
    {
        private readonly ILogger<SaveApprenticeshipFavouriteBasketCommandHandler> _logger;
        private readonly IFavouritesWriteRepository _writeRepository;

        public DeleteApprenticeshipProviderFavouriteCommandHandler(
            ILogger<SaveApprenticeshipFavouriteBasketCommandHandler> logger,
            IFavouritesReadRepository readRepository,
            IFavouritesWriteRepository writeRepository)
        {
            _logger = logger;
            _writeRepository = writeRepository;
        }

        public async Task<Unit> Handle(DeleteApprenticeshipProviderFavouriteCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling DeleteApprenticeshipProviderFavouriteCommand for Employer Account ID: {employerAccountId}, apprenticeship Id: {apprenticeshipId}, Ukprn:{ukprn}", request.EmployerAccountId,request.ApprenticeshipId,request.Ukprn);

            await _writeRepository.DeleteApprenticeshipProviderFavourites(request.EmployerAccountId, request.ApprenticeshipId,request.Ukprn);
                _logger.LogDebug("Deleted favourite for Apprenticship Id: {basketId} and Ukprn: {ukprn} for Employer Id:{employerId}", request.ApprenticeshipId, request.Ukprn, request.EmployerAccountId);
           
            return Unit.Value;
        }
    }
}