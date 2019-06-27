using System.Collections.Generic;

namespace DfE.EmployerFavourites.Api.Models
{
    public class Favourite
    {
        public Favourite()
        {
            Ukprns = new List<int>();
        }

        public string ApprenticeshipId { get; set; }
        public IList<int> Ukprns { get; set; }
    }
}
