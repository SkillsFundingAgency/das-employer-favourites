using System.Collections.Generic;
using System.Linq;

namespace DfE.EmployerFavourites.Web.Models
{
    public class TrainingProvidersViewModel
    {
        public string EmployerAccountId { get; set; }
        public string ApprenticeshipId { get; set; }
        public ApprenticeshipFavouriteViewModel Apprenticeship { get; set; }
        public IReadOnlyList<TrainingProviderViewModel> Items { get; set; }
        public bool HasLegalEntity { get; set; }
        public bool HasInactiveProviders => Items?.Any(w => w.Active == false) ?? false;
    }
}