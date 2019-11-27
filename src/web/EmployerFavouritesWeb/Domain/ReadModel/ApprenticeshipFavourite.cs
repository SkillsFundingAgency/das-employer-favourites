using System;
using System.Collections.Generic;

namespace DfE.EmployerFavourites.Domain.ReadModel
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

        public ApprenticeshipFavourite(string apprenticeshipId, Provider provider) : this(apprenticeshipId)
        {
            Providers.Add(provider);
        }

        public ApprenticeshipFavourite(string apprenticeshipId, List<Provider> providers) : this(apprenticeshipId)
        {
            ApprenticeshipId = apprenticeshipId;
            Providers = providers;
        }

        public string ApprenticeshipId { get; set; }
        public IList<Provider> Providers { get; set; }
        public string Title { get; set; }
        public bool IsFramework => TestForFramework();
        public byte Level { get; set; }
        public byte TypicalLength { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool Active { get; set; }

        private bool TestForFramework()
        {
            if (string.IsNullOrWhiteSpace(ApprenticeshipId)) 
                throw new ArgumentException("Needs to have a value", nameof(ApprenticeshipId));

            return !int.TryParse(ApprenticeshipId, out int _);
        }
    }
}