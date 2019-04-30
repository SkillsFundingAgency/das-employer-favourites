using MediatR;

namespace DfE.EmployerFavourites.ApplicationServices.Commands
{
    public class SaveApprenticeshipFavouriteCommand : IRequest
    {
        public string UserId { get; set; }
        public string ApprenticeshipId { get; set; }
        public int? Ukprn { get; set; }
    }
}