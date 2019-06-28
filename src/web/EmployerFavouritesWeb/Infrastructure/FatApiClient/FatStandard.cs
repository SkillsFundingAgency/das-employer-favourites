using Newtonsoft.Json;

namespace DfE.EmployerFavourites.Web.Infrastructure.FatApiClient
{
    public class FatStandard
    {
        [JsonProperty("StandardId")]
        public string StandardId { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Level")]
        public byte Level { get; set; }

        [JsonProperty("Duration")]
        public byte Duration { get; set; }
    }
}
