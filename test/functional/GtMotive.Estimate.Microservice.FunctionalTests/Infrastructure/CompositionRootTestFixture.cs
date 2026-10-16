using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using GtMotive.Estimate.Microservice.Api;
using GtMotive.Estimate.Microservice.Infrastructure;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Settings;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
