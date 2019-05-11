using MediatR;

namespace DfE.EmployerFavourites.Application.Queries
{
    public class ViewEmployerFavouritesQuery : IRequest<ViewEmployerFavouritesResponse>
    {
        public string EmployerAccountId { get; set; }
    }
}