
using MediatR;

namespace DfE.EmployerFavourites.Application.Queries
{
    public class EmployerHasLegalEntityQuery : IRequest<bool>
    {
        public string EmployerAccountId { get; set; }
    }
}
