using System.Collections.Generic;

namespace DfE.EmployerFavourites.Api.Models
{
    public class Favourite
    {
        public Favourite()
        {
            Providers = new List<Provider>();
        }

        public string ApprenticeshipId { get; set; }
        public IList<Provider> Providers { get; set; }
    }
}
