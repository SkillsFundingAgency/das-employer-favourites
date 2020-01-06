using System.Collections.Generic;
using System.Linq;
using Basket = Sfa.Das.Sas.Shared.Basket.Models;

namespace DfE.EmployerFavourites.Domain.WriteModel
{
    public class ApprenticeshipFavourite
    {
        public ApprenticeshipFavourite()
        {
            Providers = new List<Provider>();
        }

        public ApprenticeshipFavourite(string apprenticeshipId) : this()
        {
            ApprenticeshipId = apprenticeshipId;
        }

        public ApprenticeshipFavourite(string apprenticeshipId, IList<Basket.Provider> providers) : this(apprenticeshipId)
        {
            if (providers != null)
                Providers = providers.Select(p => new Provider (p.Ukprn, p.Locations)).ToList();
        }

        public string ApprenticeshipId { get; set; }
        public IList<Provider> Providers { get; set; }
    }
}