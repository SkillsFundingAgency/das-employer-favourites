using System;
using System.Threading;
using System.Threading.Tasks;
using DfE.EmployerFavourites.ApplicationServices.Commands;
using DfE.EmployerFavourites.ApplicationServices.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DfE.EmployerFavourites.ApplicationServices.Queries
{
    public class GetApprenticeshipFavouriteRequestHandler : IRequestHandler<GetApprenticeshipFavouritesRequest,ApprenticeshipFavourites>
    {
        private readonly ILogger<GetApprenticeshipFavouriteRequestHandler> _logger;
        private readonly IFavouritesRepository _repository;
        private readonly IEmployerAccountRepository _employerAccountRepository;

        public GetApprenticeshipFavouriteRequestHandler(
            ILogger<GetApprenticeshipFavouriteRequestHandler> logger,
            IFavouritesRepository repository, IEmployerAccountRepository employerAccountRepository)
        {
            _logger = logger;
            _repository = repository;
            _employerAccountRepository = employerAccountRepository;
        }

        public async Task<ApprenticeshipFavourites> Handle(GetApprenticeshipFavouritesRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.EmployerAccountID))
            {
                throw new ArgumentException("Employer account Id is required but is not provided");
            }

            var favourites = (await _repository.GetApprenticeshipFavourites(request.EmployerAccountID)) ?? new ApprenticeshipFavourites();

            return favourites;
        }
    }
}