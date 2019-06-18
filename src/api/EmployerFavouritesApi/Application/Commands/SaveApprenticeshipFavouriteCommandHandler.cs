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

            var writeModel = existingFavourites.MapToWriteModel();

            writeModel.Add(request.ApprenticeshipId, request.Ukprn);

            if (writeModel.UpdateStatus != DomainUpdateStatus.NoAction)
                await _writeRepository.SaveApprenticeshipFavourites(request.EmployerAccountId, writeModel);

            return new SaveApprenticeshipFavouriteCommandResponse { CommandResult = writeModel.UpdateStatus };
        }
    }
}
