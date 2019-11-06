using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Account.Api.Client;

namespace DfE.EmployerFavourites.Application.Queries
{
    public class EmployerHasLegalEntityQueryHandler : IRequestHandler<EmployerHasLegalEntityQuery, bool>
    {
        private readonly IAccountApiClient _accountApi;
        private readonly ILogger<EmployerHasLegalEntityQueryHandler> _logger;

        public EmployerHasLegalEntityQueryHandler(
             IAccountApiClient accountApiClient,
             ILogger<EmployerHasLegalEntityQueryHandler> logger
            )
        {
            _accountApi = accountApiClient;
            _logger = logger;
        }

        public async Task<bool> Handle(EmployerHasLegalEntityQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var LegalEntities = await _accountApi.GetLegalEntitiesConnectedToAccount(request.EmployerAccountId);
               
                _logger.LogInformation((LegalEntities.Count == 1) ? $"{LegalEntities.Count} legal entity found" : $"{LegalEntities.Count} legal entities found");

                return LegalEntities.Count > 0;
            }
            catch (Exception e)
            {
                _logger.LogWarning("No legal entities found for this account.", e);
                
                return false;
            }
        }
    }
}
