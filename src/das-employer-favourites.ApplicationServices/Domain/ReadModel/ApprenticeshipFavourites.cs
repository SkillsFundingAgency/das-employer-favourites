using System.Collections.Generic;
using System.Linq;

namespace DfE.EmployerFavourites.ApplicationServices.Domain.ReadModel
{
    public class ApprenticeshipFavourites : List<ApprenticeshipFavourite>
    {
        public WriteModel.ApprenticeshipFavourites MapToWriteModel()
        {
            var model = new Domain.WriteModel.ApprenticeshipFavourites();

            model.AddRange(this.Select(x => new Domain.WriteModel.ApprenticeshipFavourite
            {
                ApprenticeshipId = x.ApprenticeshipId,
                Ukprns = x.Ukprns
            }));

            return model;
        }
    }
}