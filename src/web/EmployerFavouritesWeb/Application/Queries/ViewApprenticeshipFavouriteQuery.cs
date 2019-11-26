using MediatR;

namespace DfE.EmployerFavourites.Application.Queries
{
    public class ViewApprenticeshipFavouriteQuery : IRequest<ViewApprenticeshipFavouriteResponse>
    {
        public string EmployerAccountId { get; set; }
        public string ApprenticeshipId { get; set; }
    }
}