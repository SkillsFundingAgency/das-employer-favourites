using MediatR;

namespace DfE.EmployerFavourites.Application.Commands
{
    public class SaveApprenticeshipFavouriteCommand : IRequest<string>
    {
        public string UserId { get; set; }
        public string ApprenticeshipId { get; set; }
        public int? Ukprn { get; set; }
    }
}