using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Infrastructure.Repositories;

namespace GtMotive.Estimate.Microservice.Infrastructure
{
    /// <summary>
    /// Unit of work that persists the changes buffered by the repositories.
    /// </summary>
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly VehicleRepository _vehicleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="vehicleRepository">Vehicle repository.</param>
        public UnitOfWork(VehicleRepository vehicleRepository)
        {
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
    }
}
