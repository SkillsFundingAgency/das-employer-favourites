using System.Collections.Generic;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;

namespace DfE.EmployerFavourites.Application.Queries
{
    public class GetUserAccountsQuery : IRequest<ICollection<AccountDetailViewModel>>
    {
        public string UserId { get; }

        public GetUserAccountsQuery(string userId)
        {
            UserId = userId;
        }
    }
}