using DfE.EmployerFavourites.Web.Domain;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

namespace DfE.EmployerFavourites.Web.Infrastructure
{
    public class ApprenticeshipFavouritesEntity : TableEntity
    {
        public const string ROW_KEY = "ApprenticeshipFavourites";

        public ApprenticeshipFavouritesEntity()
        {
        }

        public ApprenticeshipFavouritesEntity(string employerAccountId, ApprenticeshipFavourites favourites)
        {
            PartitionKey = employerAccountId;
            RowKey = ROW_KEY;
            Favourites = JsonConvert.SerializeObject(favourites);
        }

        public string Favourites { get; set; }

        public ApprenticeshipFavourites ToApprenticeshipFavourites()
        {
            return JsonConvert.DeserializeObject<ApprenticeshipFavourites>(Favourites);
        }
    }
}