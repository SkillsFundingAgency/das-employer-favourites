using System.Collections.Generic;

namespace DfE.EmployerFavourites.Web.Models
{
    public class TrainingProviderViewModel
    {
        public string ProviderName { get; set; }
        public IEnumerable<char> TrainingOptions { get; set; }
        public IEnumerable<char> EmployerSatisfaction { get; set; }
        public IEnumerable<char> LearnerSatisfaction { get; set; }
        public IEnumerable<char> AcheivementRate { get; set; }
    }
}