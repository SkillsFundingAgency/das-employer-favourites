using System.Collections.Generic;

namespace DfE.EmployerFavourites.ApplicationServices.Domain.WriteModel
{
    public class ApprenticeshipFavourite
    {
        public ApprenticeshipFavourite()
        {
            Ukprns = new List<int>();
        }

        public string ApprenticeshipId { get; set; }
        public IList<int> Ukprns { get; set; }
    }
}