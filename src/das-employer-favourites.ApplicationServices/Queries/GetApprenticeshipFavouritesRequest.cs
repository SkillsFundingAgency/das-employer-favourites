using DfE.EmployerFavourites.ApplicationServices.Domain;
using MediatR;

namespace DfE.EmployerFavourites.ApplicationServices.Queries
{
    public class GetApprenticeshipFavouritesRequest : IRequest<ApprenticeshipFavourites>
    {
        public string EmployerAccountID { get; set; }
    }
}