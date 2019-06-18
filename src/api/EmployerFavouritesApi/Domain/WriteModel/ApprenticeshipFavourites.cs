using System.Collections.Generic;
using System.Linq;

namespace DfE.EmployerFavourites.Api.Domain.WriteModel
{
    public class ApprenticeshipFavourites : List<ApprenticeshipFavourite>
    {
        public DomainUpdateStatus UpdateStatus { get; private set; }

        public void Add(string apprenticeshipId, int ukprn)
        {
            if (Count == 0)
            {
                UpdateStatus = DomainUpdateStatus.Created;
                Add(new ApprenticeshipFavourite
                {
                    ApprenticeshipId = apprenticeshipId,
                    Ukprns = new List<int> { ukprn }
                });

                return;
            }

            var matchingApprenticeship = this.SingleOrDefault(x => x.ApprenticeshipId == apprenticeshipId);

            if (matchingApprenticeship != null)
            {
                if (matchingApprenticeship.Ukprns.Any(x => x == ukprn))
                {
                    UpdateStatus = DomainUpdateStatus.NoAction;
                    return;
                }

                UpdateStatus = DomainUpdateStatus.Updated;
                matchingApprenticeship.Ukprns.Add(ukprn);
            }
            else
            {
                UpdateStatus = DomainUpdateStatus.Updated;

                Add(new ApprenticeshipFavourite
                {
                    ApprenticeshipId = apprenticeshipId,
                    Ukprns = new List<int> { ukprn }
                });

                return;
            }
        }
    }
}