using System;

namespace DfE.EmployerFavourites.Web.Models
{
    public class ApprenticeshipFavouriteViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public bool IsFramework { get; set; }
        public bool HasTrainingProviders { get; set; }
        public int? Ukprn { get; set; } // Note: Will be removed when we introduce multiple training providers
        public string Level { get; set; }
        public string TypicalLength { get; set; }
        public string ExpiryDate { get; set; }
        public Uri FatUrl { get; set; }
    }
}