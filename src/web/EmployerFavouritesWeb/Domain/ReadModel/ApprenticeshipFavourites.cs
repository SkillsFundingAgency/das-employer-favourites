using System.Collections.Generic;
using System.Linq;

namespace DfE.EmployerFavourites.Domain.ReadModel
{
    public class ApprenticeshipFavourites : List<ApprenticeshipFavourite>
    {
        public WriteModel.ApprenticeshipFavourites MapToWriteModel()
        {
            var model = new Domain.WriteModel.ApprenticeshipFavourites();

            model.AddRange(this.Select(x => new Domain.WriteModel.ApprenticeshipFavourite
            {
                ApprenticeshipId = x.ApprenticeshipId,
                Providers = x.Providers.Select(y => new WriteModel.Provider(y.Ukprn, y.Locations)).ToList() ?? new List<WriteModel.Provider> { }
            })); 

            return model;
        }
    }
}