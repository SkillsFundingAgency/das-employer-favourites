using MediatR;

namespace DfE.EmployerFavourites.Application.Queries
{
    public class ViewTrainingProviderForApprenticeshipFavouriteQuery : IRequest<ViewTrainingProviderForApprenticeshipFavouriteResponse>
    {
        public string EmployerAccountId { get; set; }
        public string ApprenticeshipId { get; set; }
        public int Ukprn { get; set; }
    }
}