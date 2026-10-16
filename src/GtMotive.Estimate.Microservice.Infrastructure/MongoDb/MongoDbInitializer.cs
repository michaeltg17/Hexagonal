using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Entities;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb
{
    /// <summary>
    /// Ensures the Mongo database schema (indexes) is in place before the application handles requests.
    /// </summary>
    /// <remarks>
    /// The operation is idempotent and safe to run concurrently from multiple instances:
    /// index creation conflicts between concurrent startups are ignored. If the stored data
    /// already violates one of the enforced invariants (for example two active rentals of the
    /// same client), index creation fails and the startup fails loudly; the data must be
    /// corrected before the service can start.
    /// </remarks>
    public sealed class MongoDbInitializer
    {
        private const int MaxInitializationAttempts = 5;
        private const int IndexOptionsConflictErrorCode = 85;
        private const int IndexAlreadyExistsErrorCode = 86;
        private const string VehiclesCollectionName = "vehicles";
        private const string RentalsCollectionName = "rentals";

        private static readonly TimeSpan InitializationRetryDelay = TimeSpan.FromSeconds(2);

        private readonly IMongoDatabase _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbInitializer"/> class.
        /// </summary>
        /// <param name="client">Mongo client.</param>
        /// <param name="options">Mongo database settings.</param>
        public MongoDbInitializer(IMongoClient client, IOptions<MongoDbSettings> options)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(options);

            _database = client.GetDatabase(options.Value.MongoDbDatabaseName);
        }

        /// <summary>
        /// Creates the collections indexes. The operation is idempotent.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task InitializeAsync()
        {
            var attempt = 0;

            // The replica set may not have a writable primary yet (fresh deployment or failover).
            // The operation is idempotent, so it is safe to retry.
            while (true)
            {
                attempt++;

                try
                {
                    await InitializeVehiclesAsync();
                    await InitializeRentalsAsync();

                    return;
                }
                catch (MongoClientException) when (attempt < MaxInitializationAttempts)
                {
                    await Task.Delay(InitializationRetryDelay);
                }
            }
        }

        private static async Task CreateIndexAsync<TDocument>(
            IMongoCollection<TDocument> collection,
            CreateIndexModel<TDocument> model)
        {
            try
            {
                await collection.Indexes.CreateOneAsync(model);
            }
            catch (MongoCommandException ex) when (ex.Code is IndexOptionsConflictErrorCode or IndexAlreadyExistsErrorCode)
            {
                // Another instance created the same index concurrently.
            }
        }

        private async Task InitializeVehiclesAsync()
        {
            var vehicles = _database.GetCollection<VehicleDocument>(VehiclesCollectionName);

            var plateIndex = new CreateIndexModel<VehicleDocument>(
                new BsonDocument("Plate", 1),
                new CreateIndexOptions<VehicleDocument> { Unique = true });
            await CreateIndexAsync(vehicles, plateIndex);
        }

        private async Task InitializeRentalsAsync()
        {
            var rentals = _database.GetCollection<RentalDocument>(RentalsCollectionName);

            var activeRentalFilter = new BsonDocument("EndDate", BsonNull.Value);
            var clientIndex = new CreateIndexModel<RentalDocument>(
                new BsonDocument("Client", 1),
                new CreateIndexOptions<RentalDocument>
                {
                    Unique = true,
                    PartialFilterExpression = activeRentalFilter,
                });

            await CreateIndexAsync(rentals, clientIndex);
        }
    }
}
