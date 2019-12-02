using DfE.EmployerFavourites.Domain.ReadModel;

namespace DfE.EmployerFavourites.Application.Queries
{
    public class ViewApprenticeshipFavouriteResponse
    {
        public ApprenticeshipFavourite Favourite { get; internal set; }
        public bool HasLegalEntities { get; set; }
    }
}