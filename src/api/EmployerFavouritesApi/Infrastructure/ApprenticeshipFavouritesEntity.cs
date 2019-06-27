using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

namespace DfE.EmployerFavourites.Api.Infrastructure
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

        public Domain.WriteModel.ApprenticeshipFavourites ToApprenticeshipFavouritesWriteModel()
        {
            return JsonConvert.DeserializeObject<Domain.WriteModel.ApprenticeshipFavourites>(Favourites);
        }
    }
}