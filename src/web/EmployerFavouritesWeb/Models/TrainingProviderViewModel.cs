using System.Collections.Generic;

namespace DfE.EmployerFavourites.Web.Models
{
    public class TrainingProviderViewModel
    {
        public string ProviderName { get; set; }
        public IEnumerable<char> Phone { get; set; }
        public IEnumerable<char> Email { get; set; }
        public IEnumerable<char> Website { get; set; }
        public IEnumerable<char> EmployerSatisfaction { get; set; }
        public IEnumerable<char> LearnerSatisfaction { get; set; }
    }
}