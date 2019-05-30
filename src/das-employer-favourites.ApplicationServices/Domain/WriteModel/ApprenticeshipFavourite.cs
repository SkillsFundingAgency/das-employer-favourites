using System;
using System.Collections.Generic;

namespace DfE.EmployerFavourites.ApplicationServices.Domain.WriteModel
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
        public string Title { get; set; }

        public bool IsFramework => TestForFramework();

        private bool TestForFramework()
        {
            if (string.IsNullOrWhiteSpace(ApprenticeshipId))
                throw new ArgumentException("Needs to have a value", nameof(ApprenticeshipId));

            return !int.TryParse(ApprenticeshipId, out int _);
        }
    }
}


