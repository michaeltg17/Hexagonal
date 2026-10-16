using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Infrastructure.Repositories;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure
{
    /// <summary>
    /// Unit of work that persists the changes buffered by the repositories.
    /// </summary>
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly IMongoClient _client;
        private readonly VehicleRepository _vehicleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="client">Mongo client.</param>
        /// <param name="vehicleRepository">Vehicle repository.</param>
        public UnitOfWork(IMongoClient client, VehicleRepository vehicleRepository)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        }

        /// <summary>
        /// Applies all database changes.
        /// </summary>
        /// <returns>Number of affected rows.</returns>
        public Task<int> Save()
        {
            return _vehicleRepository.SaveChanges();
        }

        /// <summary>
        /// Applies all database changes in a single transaction.
        /// </summary>
        /// <returns>Number of affected rows.</returns>
        public async Task<int> SaveInTransaction()
        {
            using var session = await _client.StartSessionAsync();
            session.StartTransaction();

            _vehicleRepository.Session = session;

            try
            {
                var saved = await _vehicleRepository.SaveChanges();

                await session.CommitTransactionAsync();

                return saved;
            }
            catch
            {
                try
                {
                    await session.AbortTransactionAsync();
                }
                catch (MongoException)
                {
                    // The transaction may already have been aborted by the driver.
                }

                throw;
            }
            finally
            {
                _vehicleRepository.Session = null;
            }
        }
    }
}
