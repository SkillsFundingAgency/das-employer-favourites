using DfE.EmployerFavourites.Web.Infrastructure.FatApiClient;
using System;
using System.Collections.Generic;

namespace DfE.EmployerFavourites.Web.Models
{
    public class TrainingProviderViewModel
    {
        public string ProviderName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string EmployerSatisfaction { get; set; }
        public string LearnerSatisfaction { get; set; }
        public int Ukprn { get; set; }
        public Uri FatUrl { get; set; }
        public List<LocationViewModel> Locations { get; set; } 
        public string CreateVacancyUrl { get; set; }
        public AddressViewModel HeadOfficeAddress { get; set; }
        public bool Active { get; set; }
    }
}