using System;
using MediatR;

namespace DfE.EmployerFavourites.Application.Commands
{
    public class SaveApprenticeshipFavouriteBasketCommand : IRequest<string>
    {
        public string UserId { get; set; }
        public string ApprenticeshipId { get; set; }
        public int? Ukprn { get; set; }
        public Guid BasketId { get; set; }
    }
}