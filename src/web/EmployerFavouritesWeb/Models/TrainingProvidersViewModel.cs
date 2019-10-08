using System.Collections.Generic;

namespace DfE.EmployerFavourites.Web.Models
{
    public class TrainingProvidersViewModel
    {
        public string EmployerAccountId { get; set; }
        public IReadOnlyList<TrainingProviderViewModel> Items { get; set; }
        public string ApprenticeshipId { get; set; }
        public bool HasLegalEntity { get; set; }
    }
}