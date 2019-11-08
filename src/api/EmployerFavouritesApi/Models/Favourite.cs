using System.Collections.Generic;

namespace DfE.EmployerFavourites.Api.Models
{
    public class Favourite
    {
        public Favourite()
        {
            Ukprns = new List<ProviderData>();
        }

        public string ApprenticeshipId { get; set; }
        public IList<ProviderData> Ukprns { get; set; }
    }
}
