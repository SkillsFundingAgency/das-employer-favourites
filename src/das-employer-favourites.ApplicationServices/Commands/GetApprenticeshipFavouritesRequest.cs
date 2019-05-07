using DfE.EmployerFavourites.ApplicationServices.Domain;
using MediatR;

namespace DfE.EmployerFavourites.ApplicationServices.Commands
{
    public class GetApprenticeshipFavouritesRequest : IRequest<ApprenticeshipFavourites>
    {
        public string UserId { get; set; }
    }
}