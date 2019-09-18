using DfE.EmployerFavourites.Api.Domain.WriteModel;
using MediatR;

namespace DfE.EmployerFavourites.Api.Application.Commands
{
    public class DeleteApprenticeshipFavouriteCommand : IRequest<DeleteApprenticeshipFavouriteCommandResponse>
    {
        public string EmployerAccountId { get; set; }
        public string ApprenticeshipId { get; set; }
    }
}
