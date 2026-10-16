using System;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles.ValueObjects;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Rental entity.
    /// </summary>
    public abstract class Rental : IRental
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rental"/> class.
        /// </summary>
        protected Rental()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rental"/> class.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <param name="client">Client that rents the vehicle.</param>
        /// <param name="startDate">Start date.</param>
        protected Rental(VehicleId vehicleId, Client client, DateTime startDate)
            : this()
        {
            Id = new RentalId(Guid.NewGuid());
            VehicleId = vehicleId;
            Client = client;
            StartDate = startDate;
        }

        /// <summary>
        /// Gets or sets the rental identifier.
        /// </summary>
        public RentalId Id { get; protected set; }

        /// <summary>
        /// Gets or sets the vehicle identifier.
        /// </summary>
        public VehicleId VehicleId { get; protected set; }

        /// <summary>
        /// Gets or sets the client that rents the vehicle.
        /// </summary>
        public Client Client { get; protected set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTime StartDate { get; protected set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public DateTime? EndDate { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the rental is active.
        /// </summary>
        public bool IsActive => EndDate is null;

        /// <summary>
        /// Marks the rental as ended.
        /// </summary>
        /// <param name="endDate">End date.</param>
        public void End(DateTime endDate)
        {
            if (EndDate is not null)
            {
                throw new RentalAlreadyReturnedException("The rental has already been ended.");
            }

            EndDate = endDate;
        }
    }
}
