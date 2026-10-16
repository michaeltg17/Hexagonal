using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using GtMotive.Estimate.Microservice.Api;
using GtMotive.Estimate.Microservice.Infrastructure;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Entities;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Settings;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Testcontainers.MongoDb;
using Xunit;

[assembly: CLSCompliant(false)]

namespace GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure
{
    public sealed class CompositionRootTestFixture : IAsyncLifetime
    {
        private const string MongoImage = "mongo:8";
        private const int MongoPort = 27017;
        private const string ReplicaSetName = "rs0";
        private const string VehiclesCollectionName = "vehicles";
        private const string RentalsCollectionName = "rentals";
        private static readonly TimeSpan WaitStrategyTimeout = TimeSpan.FromMinutes(2);

        private readonly MongoDbContainer _container;
        private ServiceProvider _serviceProvider;

        public CompositionRootTestFixture()
        {
            _container = new MongoDbBuilder(MongoImage)
                .WithExposedPort(MongoPort)
                .WithReplicaSet(ReplicaSetName)
                .WithWaitStrategy(
                    Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(
                        MongoPort,
                        strategy => strategy.WithTimeout(WaitStrategyTimeout)))
                .Build();
        }

        public IConfiguration Configuration { get; private set; }

        /// <summary>
        /// Serializes a Guid in the same format the application persists identifiers.
        /// </summary>
        /// <param name="value">Guid value.</param>
        /// <returns>The Guid as a BSON value.</returns>
        public static BsonValue SerializeGuid(Guid value)
        {
            using var stream = new MemoryStream();

            using (var writer = new BsonBinaryWriter(stream))
            {
                writer.WriteStartDocument();
                writer.WriteName("Value");
                BsonSerializer.Serialize<Guid>(writer, value);
                writer.WriteEndDocument();

                stream.Position = 0;
            }

            using var reader = new BsonBinaryReader(stream);
            var document = BsonSerializer.Deserialize<BsonDocument>(reader);

            return document["Value"];
        }

        public async Task InitializeAsync()
        {
            await _container.StartAsync();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();
            Configuration = configuration;
            ConfigureServices(services, _container.GetConnectionString());
            services.AddSingleton<IConfiguration>(configuration);
            _serviceProvider = services.BuildServiceProvider();
        }

        public async Task DisposeAsync()
        {
            if (_serviceProvider is not null)
            {
                await _serviceProvider.DisposeAsync();
            }

            await _container.DisposeAsync();
        }

        public async Task UsingHandlerForRequest<TRequest>(Func<IRequestHandler<TRequest, Unit>, Task> handlerAction)
            where TRequest : IRequest<Unit>
        {
            ArgumentNullException.ThrowIfNull(handlerAction);

            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IRequestHandler<TRequest, Unit>>();

            await handlerAction.Invoke(handler);
        }

        public async Task UsingHandlerForRequestResponse<TRequest, TResponse>(Func<IRequestHandler<TRequest, TResponse>, Task> handlerAction)
            where TRequest : IRequest<TResponse>
        {
            ArgumentNullException.ThrowIfNull(handlerAction);

            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();

            if (handler == null)
            {
                Debug.Fail("The requested handler has not been registered");
            }

            await handlerAction.Invoke(handler);
        }

        public async Task UsingRepository<TRepository>(Func<TRepository, Task> handlerAction)
        {
            ArgumentNullException.ThrowIfNull(handlerAction);

            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<TRepository>();

            if (handler == null)
            {
                Debug.Fail("The requested handler has not been registered");
            }

            await handlerAction.Invoke(handler);
        }

        public async Task ResetDatabaseAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
            var initializer = scope.ServiceProvider.GetRequiredService<MongoDbInitializer>();

            await database.DropCollectionAsync(VehiclesCollectionName);
            await database.DropCollectionAsync(RentalsCollectionName);
            await initializer.InitializeAsync();
        }

        /// <summary>
        /// Counts the rentals that have not been returned yet.
        /// </summary>
        /// <returns>The number of active rentals.</returns>
        public async Task<int> CountActiveRentalsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();

            return (int)await database
                .GetCollection<RentalDocument>(RentalsCollectionName)
                .CountDocumentsAsync(rental => rental.EndDate == null);
        }

        /// <summary>
        /// Counts all the persisted rentals.
        /// </summary>
        /// <returns>The number of rentals.</returns>
        public async Task<int> CountRentalsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();

            return (int)await database
                .GetCollection<RentalDocument>(RentalsCollectionName)
                .CountDocumentsAsync(FilterDefinition<RentalDocument>.Empty);
        }

        /// <summary>
        /// Gets the concurrency version of a vehicle document.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <returns>The vehicle version or 0 if the vehicle does not exist.</returns>
        public async Task<long> GetVehicleVersionAsync(Guid vehicleId)
        {
            using var scope = _serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();

            var document = await database
                .GetCollection<VehicleDocument>(VehiclesCollectionName)
                .Find(vehicle => vehicle.Id == vehicleId)
                .FirstOrDefaultAsync();

            return document?.Version ?? 0;
        }

        /// <summary>
        /// Runs the Mongo database initializer again against the current database.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task InitializeDatabaseAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var initializer = scope.ServiceProvider.GetRequiredService<MongoDbInitializer>();

            return initializer.InitializeAsync();
        }

        /// <summary>
        /// Inserts a raw rental document, bypassing the application mapping.
        /// </summary>
        /// <param name="document">Rental document.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task InsertRentalDocumentAsync(BsonDocument document)
        {
            using var scope = _serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();

            await database.GetCollection<BsonDocument>(RentalsCollectionName).InsertOneAsync(document);
        }

        /// <summary>
        /// Drops a rentals collection index by name.
        /// </summary>
        /// <param name="indexName">Index name.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task DropRentalIndexAsync(string indexName)
        {
            using var scope = _serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();

            await database.GetCollection<BsonDocument>(RentalsCollectionName).Indexes.DropOneAsync(indexName);
        }

        private static void ConfigureServices(IServiceCollection services, string mongoConnectionString)
        {
            services.AddApiDependencies();
            services.AddLogging();
            services.AddBaseInfrastructure(true);

            services.Configure<MongoDbSettings>(options =>
            {
                options.ConnectionString = mongoConnectionString;
                options.MongoDbDatabaseName = "hexagonal_functional_tests";
            });
        }
    }
}
