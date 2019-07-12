using System;
using Newtonsoft.Json;

namespace DfE.EmployerFavourites.Web.Infrastructure.FatApiClient
{
    public class FatFramework
    {
        [JsonProperty("FrameworkId")]
        public string FrameworkId { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Level")]
        public byte Level { get; set; }

        [JsonProperty("Duration")]
        public byte Duration { get; set; }

        [JsonProperty("ExpiryDate")]
        public DateTime ExpiryDate { get; set; }
    }
}
