using DfE.EmployerFavourites.Api.Models;
using System.Collections.Generic;
using System.Linq;
using WriteModel = DfE.EmployerFavourites.Api.Domain.WriteModel;

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
                Providers = x?.Providers.Select(y => new WriteModel.Provider { Ukprn = y.Ukprn, LocationIds = y.LocationIds, Name = y.Name}).ToList() ?? new List<WriteModel.Provider>()
            }));

            return model;
        }

        public bool Exists(string apprenticeshipId)
        {
           return this.Any(e => e.ApprenticeshipId == apprenticeshipId);
        }

        public bool Exists(string apprenticeshipId, int ukprn)
        {
            return this.Any(s => s.ApprenticeshipId == apprenticeshipId && s.Providers.Any(a => a.Ukprn == ukprn));
        }
    }
}