using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Api.Domain;
using MediatR;

namespace DfE.EmployerFavourites.Api.Application.Commands
{
    public class SaveApprenticeshipFavouriteCommandHandler : IRequestHandler<SaveApprenticeshipFavouriteCommand, SaveApprenticeshipFavouriteCommandResponse>
    {
        private readonly IFavouritesWriteRepository _writeRepository;
        private readonly IFavouritesReadRepository _readRepository;

        public SaveApprenticeshipFavouriteCommandHandler(IFavouritesWriteRepository writeRepository, IFavouritesReadRepository readRepository)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
        }

        public async Task<SaveApprenticeshipFavouriteCommandResponse> Handle(SaveApprenticeshipFavouriteCommand request, CancellationToken cancellationToken)
        {
            var existingFavourites = await _readRepository.GetApprenticeshipFavourites(request.EmployerAccountId);

            if (existingFavourites.Count == 0)
            {
                var favourites = new Domain.WriteModel.ApprenticeshipFavourites
                {
                    new Domain.WriteModel.ApprenticeshipFavourite
                    {
                        ApprenticeshipId = request.ApprenticeshipId,
                        Ukprns = new List<int> { request.Ukprn }
                    }
                };

                await _writeRepository.SaveApprenticeshipFavourites(request.EmployerAccountId, favourites);

                return SaveApprenticeshipFavouriteCommandResponse.Created;
            }

            var writeModel = existingFavourites.MapToWriteModel();

            var matchingApprenticeship = writeModel.SingleOrDefault(x => x.ApprenticeshipId == request.ApprenticeshipId);

            if (matchingApprenticeship != null)
            {
                if (matchingApprenticeship.Ukprns.Any(x => x == request.Ukprn))
                    return SaveApprenticeshipFavouriteCommandResponse.NoAction; // No save required

                matchingApprenticeship.Ukprns.Add(request.Ukprn);
            }
            else
            {
                writeModel.Add(new Domain.WriteModel.ApprenticeshipFavourite
                {
                    ApprenticeshipId = request.ApprenticeshipId,
                    Ukprns = new List<int> { request.Ukprn }
                });
            }

            await _writeRepository.SaveApprenticeshipFavourites(request.EmployerAccountId, writeModel);

            return SaveApprenticeshipFavouriteCommandResponse.Updated;
        }
    }
}
