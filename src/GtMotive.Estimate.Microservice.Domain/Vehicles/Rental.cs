using System;
using GtMotive.Estimate.Microservice.Domain.Exceptions;
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
        protected Rental(VehicleId vehicleId, Client client)
            : this()
        {
            Id = new RentalId(Guid.NewGuid());
            VehicleId = vehicleId;
            Client = client;
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
        /// Gets or sets the rent date.
        /// </summary>
        public DateTime? RentDate { get; protected set; }

        /// <summary>
        /// Gets or sets the return date.
        /// </summary>
        public DateTime? ReturnDate { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the rental is active.
        /// </summary>
        public bool IsActive => ReturnDate is null;

        /// <summary>
        /// Marks the rental as rented.
        /// </summary>
        /// <param name="rentDate">Rent date.</param>
        public void Rent(DateTime rentDate)
        {
            RentDate = rentDate;
        }

        /// <summary>
        /// Marks the rental as returned.
        /// </summary>
        /// <param name="returnDate">Return date.</param>
        public void Return(DateTime returnDate)
        {
            if (ReturnDate is not null)
            {
                throw new RentalAlreadyReturnedException("The rental has already been returned.");
            }

            ReturnDate = returnDate;
        }
    }
}
