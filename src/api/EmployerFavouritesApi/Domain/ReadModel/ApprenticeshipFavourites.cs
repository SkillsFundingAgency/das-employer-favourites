using DfE.EmployerFavourites.Api.Models;
using System.Collections.Generic;
using System.Linq;

namespace DfE.EmployerFavourites.Api.Domain.ReadModel
{
    public class ApprenticeshipFavourites : List<ApprenticeshipFavourite>
    {
        public WriteModel.ApprenticeshipFavourites MapToWriteModel()
        {
            var model = new WriteModel.ApprenticeshipFavourites();

            model.AddRange(this.Select(x => new WriteModel.ApprenticeshipFavourite
            {
                ApprenticeshipId = x.ApprenticeshipId,
                //  Ukprns = x?.Providers.Select(y => y.Ukprn).ToList() ?? new List<ProviderData>()
                Ukprns = x?.Providers.Select(y => new ProviderData { Ukprn = y.Ukprn, LocationIds = y.LocationIds }).ToList() ?? new List<ProviderData>()
            }));;

            return model;
        }
    }
}