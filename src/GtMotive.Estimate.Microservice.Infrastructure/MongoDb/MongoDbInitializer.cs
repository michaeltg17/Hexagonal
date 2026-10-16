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
    /// Ensures the Mongo database schema (indexes and migrations) is in place before the application handles requests.
    /// </summary>
    public sealed class MongoDbInitializer
    {
        private const string VehiclesCollectionName = "vehicles";
        private const string RentalsCollectionName = "rentals";

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
        /// Creates the collections indexes and applies the pending migrations. The operation is idempotent.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task InitializeAsync()
        {
            await InitializeVehiclesAsync();
            await InitializeRentalsAsync();
        }

        private async Task InitializeVehiclesAsync()
        {
            var vehicles = _database.GetCollection<VehicleDocument>(VehiclesCollectionName);

            var missingVersion = new BsonDocument("Version", new BsonDocument("$exists", false));
            var setInitialVersion = new BsonDocument("$set", new BsonDocument("Version", 1L));
            await vehicles.UpdateManyAsync(missingVersion, setInitialVersion);

            var plateIndex = new CreateIndexModel<VehicleDocument>(
                new BsonDocument("Plate", 1),
                new CreateIndexOptions<VehicleDocument> { Unique = true });
            await vehicles.Indexes.CreateOneAsync(plateIndex);
        }

        private async Task InitializeRentalsAsync()
        {
            var rentals = _database.GetCollection<RentalDocument>(RentalsCollectionName);

            var activeRentalFilter = new BsonDocument("ReturnDate", BsonNull.Value);
            var personIndex = new CreateIndexModel<RentalDocument>(
                new BsonDocument("Person", 1),
                new CreateIndexOptions<RentalDocument>
                {
                    Unique = true,
                    PartialFilterExpression = activeRentalFilter,
                });

            await rentals.Indexes.CreateOneAsync(personIndex);
        }
    }
}
