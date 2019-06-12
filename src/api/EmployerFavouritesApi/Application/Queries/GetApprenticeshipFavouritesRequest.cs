using DfE.EmployerFavourites.Api.Domain.ReadModel;
using MediatR;

namespace DfE.EmployerFavourites.Api.Application.Queries
{
    public class GetApprenticeshipFavouritesRequest : IRequest<ApprenticeshipFavourites>
    {
        public string EmployerAccountID { get; set; }
    }
}