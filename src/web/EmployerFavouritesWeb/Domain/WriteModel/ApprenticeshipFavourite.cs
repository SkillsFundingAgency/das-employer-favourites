using System.Collections.Generic;
using System.Linq;

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

        public ApprenticeshipFavourite(string apprenticeshipId, IDictionary<int,IList<int>> providers) : this(apprenticeshipId)
        {
            if (providers != null)
                Providers = providers.Select(p => new Provider (p.Key,p.Value )).ToList();
        }

        //public ApprenticeshipFavourite(string apprenticeshipId, Provider provider) : this(apprenticeshipId)
        //{
        //    Providers.Add(new Provider(provider.Ukprn,provider.Locations));
        //}

        public string ApprenticeshipId { get; set; }
        public IList<Provider> Providers { get; set; }
    }
}