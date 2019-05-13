using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

namespace DfE.EmployerFavourites.Infrastructure
{
    public class ApprenticeshipFavouritesEntity : TableEntity
    {
        public const string ROW_KEY = "ApprenticeshipFavourites";

        public ApprenticeshipFavouritesEntity()
        {
        }

        public ApprenticeshipFavouritesEntity(string employerAccountId, Domain.WriteModel.ApprenticeshipFavourites favourites)
        {
            PartitionKey = employerAccountId;
            RowKey = ROW_KEY;
            Favourites = JsonConvert.SerializeObject(favourites);
        }

        public string Favourites { get; set; }

        public Domain.ReadModel.ApprenticeshipFavourites ToApprenticeshipFavourites()
        {
            return JsonConvert.DeserializeObject<Domain.ReadModel.ApprenticeshipFavourites>(Favourites);
        }
    }
}