using MediatR;

namespace DfE.EmployerFavourites.Web.Queries
{
    public class ViewEmployerFavouritesQuery : IRequest<ViewEmployerFavouritesResponse>
    {
        public string EmployerAccountId { get; set; }
    }
}