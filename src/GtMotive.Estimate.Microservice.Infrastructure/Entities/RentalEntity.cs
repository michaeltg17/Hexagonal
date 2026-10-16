using System;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.Domain.Vehicles.ValueObjects;

namespace GtMotive.Estimate.Microservice.Infrastructure.Entities
{
    /// <summary>
    /// Concrete rental entity.
    /// </summary>
    public sealed class RentalEntity : Rental
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RentalEntity"/> class.
        /// </summary>
        public RentalEntity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RentalEntity"/> class.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <param name="client">Client that rents the vehicle.</param>
        /// <param name="startDate">Start date.</param>
        public RentalEntity(VehicleId vehicleId, Client client, DateTime startDate)
            : base(vehicleId, client, startDate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RentalEntity"/> class from persisted data.
        /// </summary>
        /// <param name="id">Rental identifier.</param>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <param name="client">Client that rents the vehicle.</param>
        /// <param name="startDate">Start date.</param>
        /// <param name="endDate">End date.</param>
        internal RentalEntity(RentalId id, VehicleId vehicleId, Client client, DateTime startDate, DateTime? endDate)
            : base(vehicleId, client, startDate)
        {
            Id = id;
            EndDate = endDate;
        }
    }
}
