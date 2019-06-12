using DfE.EmployerFavourites.ApplicationServices.Domain;
using DfE.EmployerFavourites.ApplicationServices.Domain.ReadModel;
using MediatR;

namespace DfE.EmployerFavourites.ApplicationServices.Queries
{
    public class GetApprenticeshipFavouritesRequest : IRequest<ApprenticeshipFavourites>
    {
        public string EmployerAccountID { get; set; }
    }
}