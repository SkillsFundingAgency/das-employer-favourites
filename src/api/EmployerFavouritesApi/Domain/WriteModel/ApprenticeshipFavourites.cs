using System.Collections.Generic;
using System.Linq;

namespace DfE.EmployerFavourites.Api.Domain.WriteModel
{
    public class ApprenticeshipFavourites : List<ApprenticeshipFavourite>
    {

        public void Remove(string apprenticeshipId)
        {
            RemoveAll(item => item.ApprenticeshipId == apprenticeshipId);
        }

        public void Remove(string apprenticeshipId, int ukprn)
        {
            var provider = this.FirstOrDefault(w => w.ApprenticeshipId == apprenticeshipId)?.Providers;

            provider.Remove(provider.SingleOrDefault(w => w.Ukprn == ukprn));
        }
    }
}