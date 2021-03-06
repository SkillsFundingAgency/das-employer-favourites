using System;
using System.Linq;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Api.Infrastructure.Configuration;
using DfE.EmployerFavourites.Api.Domain;
using DfE.EmployerFavourites.Api.Infrastructure.Interfaces;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System.Collections.Generic;
using DfE.EmployerFavourites.Api.Models;
using ReadModel = DfE.EmployerFavourites.Api.Domain.ReadModel;
using WriteModel = DfE.EmployerFavourites.Api.Domain.WriteModel;

namespace DfE.EmployerFavourites.Api.Infrastructure
{
    public class FatFavouritesTableStorageRepository : IFavouritesReadRepository, IFavouritesWriteRepository
    {
        private const string TABLE_NAME = "EmployerFavourites";
        private readonly ILogger<FatFavouritesTableStorageRepository> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly CloudStorageAccount _storageAccount;
        
        public FatFavouritesTableStorageRepository(
            ILogger<FatFavouritesTableStorageRepository> logger,
            IOptions<ConnectionStrings> option,
            IFatRepository fatRepository)
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

        public async Task<ReadModel.ApprenticeshipFavourites> GetApprenticeshipFavourites(string employerAccountId)
        {
            try
            {
                var table = await GetTable();
                var retrieveOperation = TableOperation.Retrieve<ApprenticeshipFavouritesEntity>(employerAccountId, ApprenticeshipFavouritesEntity.ROW_KEY);

                TableResult result = await _retryPolicy.ExecuteAsync(context => table.ExecuteAsync(retrieveOperation), new Context(nameof(GetApprenticeshipFavourites)));
                var entity = result.Result as ApprenticeshipFavouritesEntity;

                if (entity != null)
                {
                    _logger.LogTrace("\t{0}\t{1}\t{2}", entity.PartitionKey, entity.RowKey, JsonConvert.SerializeObject(entity.Favourites));
                }

                var favouritesFromTableStorage = entity?.ToApprenticeshipFavouritesWriteModel() ?? new WriteModel.ApprenticeshipFavourites();

                var favourites = await BuildReadModel(favouritesFromTableStorage);

                return favourites;
            }
            catch (StorageException ex)
            {
                _logger.LogError(ex, "Unable to Get Apprenticeship Favourites");
                throw;
            }
        }

        private async Task<ReadModel.ApprenticeshipFavourites> BuildReadModel(Domain.WriteModel.ApprenticeshipFavourites favouritesFromTableStorage)
        {
            var buildFavouritesTasks = favouritesFromTableStorage.Select(BuildReadModelItem).ToList();

            await Task.WhenAll(buildFavouritesTasks);

            var favourites = new ReadModel.ApprenticeshipFavourites();
            favourites.AddRange(buildFavouritesTasks.Select(x => x.Result));

            return favourites;
        }

        private async Task<Domain.ReadModel.ApprenticeshipFavourite> BuildReadModelItem(Domain.WriteModel.ApprenticeshipFavourite src)
        {
            return new Domain.ReadModel.ApprenticeshipFavourite
            {
                ApprenticeshipId = src.ApprenticeshipId,
                Providers = await Task.WhenAll(src.Providers?.Select(async x => new ReadModel.Provider
                {
                    Name = x.Name,
                    Ukprn = x.Ukprn,
                    LocationIds = x.LocationIds
                }))
            };
        }

        public async Task SaveApprenticeshipFavourites(string employerAccountId, Domain.WriteModel.ApprenticeshipFavourites apprenticeshipFavourite)
        {
            if (apprenticeshipFavourite == null)
            {
                throw new ArgumentNullException(nameof(apprenticeshipFavourite));
            }

            var providers = apprenticeshipFavourite.SelectMany(s => s.Providers);

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