using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DfE.EmployerFavourites.ApplicationServices.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DfE.EmployerFavourites.ApplicationServices.Commands
{
    public class SaveApprenticeshipFavouriteCommandHandler : AsyncRequestHandler<SaveApprenticeshipFavouriteCommand>
    {
        private readonly ILogger<SaveApprenticeshipFavouriteCommandHandler> _logger;
        private readonly IFavouritesRepository _repository;
        private readonly IEmployerAccountRepository _employerAccountRepository;

        public SaveApprenticeshipFavouriteCommandHandler(
            ILogger<SaveApprenticeshipFavouriteCommandHandler> logger,
            IFavouritesRepository repository, IEmployerAccountRepository employerAccountRepository)
        {
            _logger = logger;
            _repository = repository;
            _employerAccountRepository = employerAccountRepository;
        }

        protected override async Task Handle(SaveApprenticeshipFavouriteCommand request, CancellationToken cancellationToken)
        {
            var employerAccountId = await _employerAccountRepository.GetEmployerAccountId(request.UserId);
            var favourites = (await _repository.GetApprenticeshipFavourites(employerAccountId)) ?? new ApprenticeshipFavourites();

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

            await _repository.SaveApprenticeshipFavourites(employerAccountId, favourites);
        }

    }
}