using MediatR;

namespace DfE.EmployerFavourites.Api.Application.Commands
{
    public class DeleteProviderFavouriteCommand : IRequest<DeleteProviderFavouriteCommandResponse>
    {
        public string EmployerAccountId { get; set; }
        public string ApprenticeshipId { get; set; }
        public int Ukprn { get; set; }
    }
}
