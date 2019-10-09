using MediatR;

namespace DfE.EmployerFavourites.Application.Commands
{
    public class DeleteApprenticeshipFavouriteCommand : IRequest
    {
        public string ApprenticeshipId { get; set; }
        public string EmployerAccountId { get; set; }
    }
}