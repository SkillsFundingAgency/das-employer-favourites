using MediatR;

namespace DfE.EmployerFavourites.Application.Commands
{
    public class DeleteApprenticeshipProviderFavouriteCommand : IRequest
    {
        public string ApprenticeshipId { get; set; }
        public string EmployerAccountId { get; set; }
        public int Ukprn { get; set; }
    }
}