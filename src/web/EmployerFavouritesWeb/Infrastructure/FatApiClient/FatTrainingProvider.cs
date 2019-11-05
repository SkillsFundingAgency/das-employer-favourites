using System;
using System.Collections.Generic;
using DfE.EmployerFavourites.Web.Models;
using Newtonsoft.Json;

namespace DfE.EmployerFavourites.Web.Infrastructure.FatApiClient
{
    public class FatTrainingProvider
    {
        [JsonProperty("Ukprn")]
        public long Ukprn { get; set; }

        [JsonProperty("ProviderName")]
        public string ProviderName { get; set; }

        [JsonProperty("Phone")]
        public string Phone { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("Website")]
        public Uri Website { get; set; }

        [JsonProperty("EmployerSatisfaction")]
        public double EmployerSatisfaction { get; set; }

        [JsonProperty("LearnerSatisfaction")]
        public double LearnerSatisfaction { get; set; }

        [JsonProperty("Locations")]
        public List<LocationViewModel> Locations { get; set; }
    }
}
