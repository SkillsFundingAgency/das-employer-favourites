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
            this.Where(w => w.ApprenticeshipId == apprenticeshipId).ToList().ForEach(item => item.Ukprns.Remove(ukprn));
        }




    }
}