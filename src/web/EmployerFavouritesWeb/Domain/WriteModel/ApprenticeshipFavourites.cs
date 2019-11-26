using System.Collections.Generic;
using System.Linq;

namespace DfE.EmployerFavourites.Domain.WriteModel
{
    public class ApprenticeshipFavourites : List<ApprenticeshipFavourite>
    {
        public bool Update(string apprenticeshipId, IList<int> ukprns)
        {
            var existing = this.SingleOrDefault(x => x.ApprenticeshipId == apprenticeshipId);

            if (existing == null)
            {
                if (ukprns == null || ukprns.Count == 0)
                {
                    Add(new ApprenticeshipFavourite(apprenticeshipId));
                    return true;
                }
                else
                {
                    Add(new ApprenticeshipFavourite(apprenticeshipId, ukprns));
                    return true;
                }
            }
            else
            {
                if (ukprns != null || ukprns.Count > 0)
                {
                    var changeMade = false;

                    foreach (var ukprn in ukprns)
                    {
                        if (!existing.Ukprns.Contains(ukprn))
                        {
                            existing.Ukprns.Add(ukprn);

                            changeMade = true;
                        }
                    }

                    return changeMade;
                }
            }

            return false;
        }
    }
}