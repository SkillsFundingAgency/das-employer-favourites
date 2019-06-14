using System.Collections.Generic;
using System.Linq;
using DfE.EmployerFavourites.Api.Application.Commands;

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

                matchingApprenticeship.Ukprns.Add(ukprn);
            }
            else
            {
                Add(new ApprenticeshipFavourite
                {
                    ApprenticeshipId = apprenticeshipId,
                    Ukprns = new List<int> { ukprn }
                });

                UpdateStatus = DomainUpdateStatus.Updated;

                return;
            }
        }
    }
}