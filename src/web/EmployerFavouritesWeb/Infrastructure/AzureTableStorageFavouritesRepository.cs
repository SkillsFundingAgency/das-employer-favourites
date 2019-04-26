using System;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Web.Configuration;
using DfE.EmployerFavourites.Web.Domain;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DfE.EmployerFavourites.Web.Infrastructure
{
    public class AzureTableStorageFavouritesRepository : IFavouritesRepository
    {
        private const string TABLE_NAME = "EmployerFavourites";
        private readonly ILogger<AzureTableStorageFavouritesRepository> _logger;
        private readonly CloudStorageAccount _storageAccount;

        public AzureTableStorageFavouritesRepository(ILogger<AzureTableStorageFavouritesRepository> logger, IOptions<ConnectionStrings> option)
        {
            
            _logger = logger;
            
            try
            {
                _storageAccount = CloudStorageAccount.Parse(option.Value.AzureStorage);
            }
            catch (FormatException)
            {
                _logger.LogError("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
                throw;
            }
            catch (ArgumentException)
            {
                _logger.LogError("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                throw;
            }
        }
        public async Task<ApprenticeshipFavourites> GetApprenticeshipFavourites(string employerAccountId)
        {
            try
            {
                var table = await GetTable();
                TableOperation retrieveOperation = TableOperation.Retrieve<ApprenticeshipFavouritesEntity>(employerAccountId, ApprenticeshipFavouritesEntity.ROW_KEY);
                TableResult result = await table.ExecuteAsync(retrieveOperation);
                ApprenticeshipFavouritesEntity entity = result.Result as ApprenticeshipFavouritesEntity;
                if (entity != null)
                {
                    _logger.LogDebug("\t{0}\t{1}\t{2}", entity.PartitionKey, entity.RowKey, JsonConvert.SerializeObject(entity.Favourites));
                }

                // Get the request units consumed by the current operation. RequestCharge of a TableResult is only applied to Azure CosmoS DB 
                if (result.RequestCharge.HasValue)
                {
                    _logger.LogDebug("Request Charge of Retrieve Operation: {requestCharge}", result.RequestCharge);
                }

                return entity?.ToApprenticeshipFavourites();
            }
            catch (StorageException ex)
            {
                _logger.LogError(ex, "Unable to Get Apprenticeship Favourites");
                throw;
            }
        }

        public async Task SaveApprenticeshipFavourites(string employerAccountId, ApprenticeshipFavourites apprenticeshipFavourite)
        {
            if (apprenticeshipFavourite == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                var table = await GetTable();
                var entity = new ApprenticeshipFavouritesEntity(employerAccountId, apprenticeshipFavourite);

                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation.
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                ApprenticeshipFavouritesEntity insertedCustomer = result.Result as ApprenticeshipFavouritesEntity;
                    
                // Get the request units consumed by the current operation. RequestCharge of a TableResult is only applied to Azure CosmoS DB 
                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of InsertOrMerge Operation: " + result.RequestCharge);
                }
            }
            catch (StorageException ex)
            {
                 _logger.LogError(ex, "Unable to Save Apprenticeship Favourites");
                throw;
            }
        }

        private async Task<CloudTable> GetTable()
        {
            CloudTableClient tableClient = _storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            CloudTable table = tableClient.GetTableReference(TABLE_NAME);

            if (await table.CreateIfNotExistsAsync())
            {
                _logger.LogDebug($"Created Table named: {TABLE_NAME}");
            }
            else
            {
                _logger.LogDebug($"Table {TABLE_NAME} already exists");
            }

            return table;
        }
    }
}