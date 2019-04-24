using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Web.Domain;
using MediatR;

namespace DfE.EmployerFavourites.Web.Commands
{
    public class SaveApprenticeshipFavouriteCommandHandler : AsyncRequestHandler<SaveApprenticeshipFavouriteCommand>
    {
        private readonly IFavouritesRepository _repository;

        public SaveApprenticeshipFavouriteCommandHandler(IFavouritesRepository repository)
        {
            _repository = repository;
        }

        protected override async Task Handle(SaveApprenticeshipFavouriteCommand request, CancellationToken cancellationToken)
        {
            var favourites = (await _repository.GetApprenticeshipFavourites(request.EmployerAccountId)) ?? new ApprenticeshipFavourites();

            var existing = favourites.SingleOrDefault(x => x.ApprenticeshipId == request.ApprenticeshipId);

            if (existing == null)
            {
                if (request.Ukprn.HasValue)
                    favourites.Add(new ApprenticeshipFavourite(request.ApprenticeshipId, request.Ukprn.Value));
                else
                    favourites.Add(new ApprenticeshipFavourite(request.ApprenticeshipId));
            }
            else
            {
                if (!request.Ukprn.HasValue)
                    return;
                else if (existing.Ukprns.Contains(request.Ukprn.Value))
                    return;
                else    
                    existing.Ukprns.Add(request.Ukprn.Value);
            }

            await _repository.SaveApprenticeshipFavourites(request.EmployerAccountId, favourites);
        }
    }
}