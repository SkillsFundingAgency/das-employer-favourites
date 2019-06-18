using System.Threading;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Api.Domain;
using DfE.EmployerFavourites.Api.Domain.WriteModel;
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
            var hasExistingRecord = existingFavourites != null && existingFavourites.Count > 0;

            await _writeRepository.SaveApprenticeshipFavourites(request.EmployerAccountId, request.Favourites);

            return new SaveApprenticeshipFavouriteCommandResponse { CommandResult = hasExistingRecord ? DomainUpdateStatus.Updated : DomainUpdateStatus.Created };
        }
    }
}
