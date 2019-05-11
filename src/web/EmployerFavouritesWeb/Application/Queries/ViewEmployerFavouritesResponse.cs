using DfE.EmployerFavourites.Domain;

namespace DfE.EmployerFavourites.Application.Queries
{
    public class ViewEmployerFavouritesResponse
    {
        public EmployerAccount EmployerAccount { get; internal set; }
        public ApprenticeshipFavourites EmployerFavourites { get; internal set; }
    }
}