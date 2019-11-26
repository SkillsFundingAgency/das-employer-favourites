using System.Collections.Generic;

namespace DfE.EmployerFavourites.Api.Models
{
    public class Favourite
    {
        public Favourite()
        {
            Ukprns = new List<Provider>();
        }

        public string ApprenticeshipId { get; set; }
        public IList<Provider> Ukprns { get; set; }
    }
}
