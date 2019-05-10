using System.Collections.Generic;

namespace DfE.EmployerFavourites.Web.Domain
{
    public class ApprenticeshipFavourite
    {
        public ApprenticeshipFavourite()
        {
            Ukprns = new List<int>();
        }

        public ApprenticeshipFavourite(string apprenticeshipId) : this()
        {
            ApprenticeshipId = apprenticeshipId;
        }

        public ApprenticeshipFavourite(string apprenticeshipId, int ukprn) : this(apprenticeshipId)
        {
            Ukprns.Add(ukprn);
        }

        public string ApprenticeshipId { get; set; }
        public IList<int> Ukprns { get; set; }
    }
}