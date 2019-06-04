using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;

namespace DfE.EmployerFavourites.IntegrationTests.Stubs
{
    internal class StubAccountApiClient : IAccountApiClient
    {
        public Task<AccountDetailViewModel> GetAccount(string hashedAccountId)
        {
            return Task.FromResult(new AccountDetailViewModel
            {
                AccountId = 123456,
                HashedAccountId = hashedAccountId,
                DasAccountName = "Test Account"
            });
        }

        public Task<AccountDetailViewModel> GetAccount(long accountId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<TeamMemberViewModel>> GetAccountUsers(string accountId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<TeamMemberViewModel>> GetAccountUsers(long accountId)
        {
            throw new NotImplementedException();
        }

        public Task<EmployerAgreementView> GetEmployerAgreement(string accountId, string legalEntityId, string agreementId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<ResourceViewModel>> GetLegalEntitiesConnectedToAccount(string accountId)
        {
            throw new NotImplementedException();
        }

        public Task<LegalEntityViewModel> GetLegalEntity(string accountId, long id)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<LevyDeclarationViewModel>> GetLevyDeclarations(string accountId)
        {
            throw new NotImplementedException();
        }

        public Task<PagedApiResponseViewModel<AccountLegalEntityViewModel>> GetPageOfAccountLegalEntities(int pageNumber = 1, int pageSize = 1000)
        {
            throw new NotImplementedException();
        }

        public Task<PagedApiResponseViewModel<AccountWithBalanceViewModel>> GetPageOfAccounts(int pageNumber = 1, int pageSize = 1000, DateTime? toDate = null)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<ResourceViewModel>> GetPayeSchemesConnectedToAccount(string accountId)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetResource<T>(string uri) where T : IAccountResource
        {
            throw new NotImplementedException();
        }

        public Task<StatisticsViewModel> GetStatistics()
        {
            throw new NotImplementedException();
        }

        public Task<TransactionsViewModel> GetTransactions(string accountId, int year, int month)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<TransactionSummaryViewModel>> GetTransactionSummary(string accountId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<TransferConnectionViewModel>> GetTransferConnections(string accountHashedId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<AccountDetailViewModel>> GetUserAccounts(string userId)
        {
            return Task.FromResult(new List<AccountDetailViewModel>
            {
                new AccountDetailViewModel
                {
                    AccountId = 123456,
                    HashedAccountId = "ABC123"
                }
            } as ICollection<AccountDetailViewModel>);
        }
    }
}