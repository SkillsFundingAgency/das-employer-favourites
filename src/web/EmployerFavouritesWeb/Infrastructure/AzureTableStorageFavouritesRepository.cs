using System;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Domain;
using DfE.EmployerFavourites.Infrastructure.Configuration;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace DfE.EmployerFavourites.Infrastructure
{
    // TODO: This class will become obsolete when the API supports create.
    public class AzureTableStorageFavouritesRepository : IFavouritesRepository
    {
        private const string TABLE_NAME = "EmployerFavourites";
        private readonly ILogger<AzureTableStorageFavouritesRepository> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly CloudStorageAccount _storageAccount;

        public AzureTableStorageFavouritesRepository(ILogger<AzureTableStorageFavouritesRepository> logger, IOptions<ConnectionStrings> option)
        {
            
            _logger = logger;
            _retryPolicy = GetRetryPolicy();
            
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
        public Task<ApprenticeshipFavourites> GetApprenticeshipFavourites(string employerAccountId)
        {
            throw new NotSupportedException();
        }

        public async Task SaveApprenticeshipFavourites(string employerAccountId, ApprenticeshipFavourites apprenticeshipFavourite)
        {
            if (apprenticeshipFavourite == null)
            {
                throw new ArgumentNullException(nameof(apprenticeshipFavourite));
            }

            try
            {
                var table = await GetTable();
                var entity = new ApprenticeshipFavouritesEntity(employerAccountId, apprenticeshipFavourite);

                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation.
                TableResult result = await _retryPolicy.ExecuteAsync(context => table.ExecuteAsync(insertOrMergeOperation), new Context(nameof(SaveApprenticeshipFavourites)));
                ApprenticeshipFavouritesEntity insertedCustomer = result.Result as ApprenticeshipFavouritesEntity;
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
                    
            var createdTable = await _retryPolicy.ExecuteAsync(context => table.CreateIfNotExistsAsync(), new Context(nameof(GetTable)));
            
            if (createdTable)
            {
                _logger.LogDebug($"Created Table named: {TABLE_NAME}");
            }
            else
            {
                _logger.LogDebug($"Table {TABLE_NAME} already exists");
            }

            return table;
        }

        private Polly.Retry.AsyncRetryPolicy GetRetryPolicy()
        {
            return Policy
                    .Handle<StorageException>()
                    .WaitAndRetryAsync(new[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(4)
                    }, (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Error executing Table Storage command  {context.OperationKey} Reason: {exception.Message}. Retrying in {timeSpan.Seconds} secs...attempt: {retryCount}");
                    });
        }
    }
}