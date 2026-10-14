using System;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.ValueObjects;

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
        /// <param name="person">Person that rents the vehicle.</param>
        public RentalEntity(VehicleId vehicleId, Person person)
            : base(vehicleId, person)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RentalEntity"/> class from persisted data.
        /// </summary>
        /// <param name="id">Rental identifier.</param>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <param name="person">Person that rents the vehicle.</param>
        /// <param name="rentDate">Rent date.</param>
        /// <param name="returnDate">Return date.</param>
        internal RentalEntity(RentalId id, VehicleId vehicleId, Person person, DateTime? rentDate, DateTime? returnDate)
            : base(vehicleId, person)
        {
            Id = id;
            RentDate = rentDate;
            ReturnDate = returnDate;
        }
    }
}
