using DfE.EmployerFavourites.Domain.ReadModel;

namespace DfE.EmployerFavourites.Application.Queries
{
    public class ViewTrainingProviderForApprenticeshipFavouriteResponse
    {
        public EmployerAccount EmployerAccount { get; internal set; }
        public ApprenticeshipFavourites EmployerFavourites { get; internal set; }
    }
}