using DfE.EmployerFavourites.Domain;
using DfE.EmployerFavourites.Domain.ReadModel;

namespace DfE.EmployerFavourites.Application.Queries
{
    public class ViewEmployerFavouritesResponse
    {
        public EmployerAccount EmployerAccount { get; internal set; }
        public ApprenticeshipFavourites EmployerFavourites { get; internal set; }
    }
}