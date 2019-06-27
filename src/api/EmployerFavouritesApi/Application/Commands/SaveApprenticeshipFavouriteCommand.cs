using DfE.EmployerFavourites.Api.Domain.WriteModel;
using MediatR;

namespace DfE.EmployerFavourites.Api.Application.Commands
{
    public class SaveApprenticeshipFavouriteCommand : IRequest<SaveApprenticeshipFavouriteCommandResponse>
    {
        public string EmployerAccountId { get; set; }
        public ApprenticeshipFavourites Favourites { get; set; }
    }
}
