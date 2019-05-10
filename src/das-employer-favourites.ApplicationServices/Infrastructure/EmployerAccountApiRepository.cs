using System;
using System.Linq;
using System.Threading.Tasks;
using DfE.EmployerFavourites.ApplicationServices.Domain;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Account.Api.Client;

namespace DfE.EmployerFavourites.ApplicationServices.Commands
{
    public class EmployerAccountApiRepository : IEmployerAccountRepository
    {
        private readonly ILogger<EmployerAccountApiRepository> _logger;
        private readonly IAccountApiClient _accountApiClient;

        public EmployerAccountApiRepository(IAccountApiClient accountApiClient, ILogger<EmployerAccountApiRepository> logger)
        {
            _accountApiClient = accountApiClient;
            _logger = logger;
        }

        public async Task<string> GetEmployerAccountId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User Id is required but is not provided");
            }
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