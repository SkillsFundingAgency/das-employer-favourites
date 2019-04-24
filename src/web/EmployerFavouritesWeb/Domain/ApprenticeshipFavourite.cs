using System.Collections.Generic;

namespace DfE.EmployerFavourites.Web.Domain
{
    public class ApprenticeshipFavourite
    {
        public ApprenticeshipFavourite(string apprenticeshipId)
        {
            ApprenticeshipId = apprenticeshipId;
            Ukprns = new List<int>();
        }

        public ApprenticeshipFavourite(string apprenticeshipId, int ukprn) : this(apprenticeshipId)
        {
            Ukprns.Add(ukprn);
        }

        public string ApprenticeshipId { get; private set; }
        public IList<int> Ukprns { get; private set; }
    }
}