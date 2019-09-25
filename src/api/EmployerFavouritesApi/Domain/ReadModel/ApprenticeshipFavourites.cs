using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query.Expressions;

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
                Ukprns = x?.Providers.Select(y => y.Ukprn).ToList() ?? new List<int>(0)
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