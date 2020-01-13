using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;

namespace DfE.EmployerFavourites.Application.Queries
{
    public class GetUserAccountsQueryHandler : IRequestHandler<GetUserAccountsQuery, ICollection<AccountDetailViewModel>>
    {
        private readonly IAccountApiClient _accountApiClient;

        public GetUserAccountsQueryHandler(IAccountApiClient accountApiClient)
        {
            _accountApiClient = accountApiClient;
        }
        
        public async Task<ICollection<AccountDetailViewModel>> Handle(GetUserAccountsQuery request, CancellationToken cancellationToken)
        {
            return await _accountApiClient.GetUserAccounts(request.UserId);
        }
    }
}