using System.Collections.Generic;

namespace DfE.EmployerFavourites.ApplicationServices.Domain.ReadModel
{
    public class ApprenticeshipFavourite
    {
        public ApprenticeshipFavourite()
        {
            Providers = new List<Provider>();
        }

        public ApprenticeshipFavourite(string apprenticeshipId, string title) : this()
        {
            ApprenticeshipId = apprenticeshipId;
            Title = title;
        }

        public string ApprenticeshipId { get; set; }
        public IList<Provider> Providers { get; set; }
        public string Title { get; set; }
    }
}


