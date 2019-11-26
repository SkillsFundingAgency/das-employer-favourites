using DfE.EmployerFavourites.Api.Domain.WriteModel;

namespace DfE.EmployerFavourites.Api.Application.Commands
{
    public class DeleteProviderFavouriteCommandResponse
    {
        public DomainUpdateStatus CommandResult { get; set; }
    }
}