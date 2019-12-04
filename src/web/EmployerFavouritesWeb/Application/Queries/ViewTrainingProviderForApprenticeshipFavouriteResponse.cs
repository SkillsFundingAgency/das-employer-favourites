using DfE.EmployerFavourites.Domain.ReadModel;

namespace DfE.EmployerFavourites.Application.Queries
{
    public class ViewTrainingProviderForApprenticeshipFavouriteResponse
    {
        public ApprenticeshipFavourite Favourite { get; internal set; }
        public bool HasLegalEntities { get; set; }
        public Provider Provider { get; set; }
    }
}