using System;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles.ValueObjects;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Vehicle aggregate root.
    /// </summary>
    public abstract class Vehicle : IVehicle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vehicle"/> class.
        /// </summary>
        protected Vehicle()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vehicle"/> class.
        /// </summary>
        /// <param name="plate">Plate.</param>
        /// <param name="make">Make.</param>
        /// <param name="model">Model.</param>
        /// <param name="manufactureDate">Manufacture date.</param>
        protected Vehicle(Plate plate, Make make, Model model, ManufactureDate manufactureDate)
            : this()
        {
            Id = new VehicleId(Guid.NewGuid());
            Plate = plate;
            Make = make;
            Model = model;
            ManufactureDate = manufactureDate;
        }

        /// <summary>
        /// Gets or sets the vehicle identifier.
        /// </summary>
        public VehicleId Id { get; protected set; }

        /// <summary>
        /// Gets or sets the plate.
        /// </summary>
        public Plate Plate { get; protected set; }

        /// <summary>
        /// Gets or sets the make.
        /// </summary>
        public Make Make { get; protected set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public Model Model { get; protected set; }

        /// <summary>
        /// Gets or sets the manufacture date.
        /// </summary>
        public ManufactureDate ManufactureDate { get; protected set; }

        /// <summary>
        /// Gets or sets the identifier of the active rental, if the vehicle is rented.
        /// </summary>
        public RentalId? ActiveRentalId { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the vehicle is available.
        /// </summary>
        public bool IsAvailable => ActiveRentalId is null;

        /// <summary>
        /// Rents the vehicle to a client.
        /// </summary>
        /// <param name="rentalFactory">Rental factory.</param>
        /// <param name="client">Client that rents the vehicle.</param>
        /// <param name="rentDate">Rent date.</param>
        /// <returns>The created rental.</returns>
        public IRental Rent(IRentalFactory rentalFactory, Client client, DateTime rentDate)
        {
            ArgumentNullException.ThrowIfNull(rentalFactory);

            if (ActiveRentalId is not null)
            {
                throw new VehicleAlreadyRentedException("The vehicle is already rented.");
            }

            var rental = rentalFactory.NewRental(Id, client);
            rental.Rent(rentDate);
            ActiveRentalId = rental.Id;

            return rental;
        }

        /// <summary>
        /// Returns the vehicle.
        /// </summary>
        /// <param name="activeRental">The active rental of the vehicle.</param>
        /// <param name="returnDate">Return date.</param>
        /// <returns>The returned rental.</returns>
        public IRental Return(IRental activeRental, DateTime returnDate)
        {
            ArgumentNullException.ThrowIfNull(activeRental);

            if (activeRental.Id != ActiveRentalId)
            {
                throw new VehicleNotRentedException("The vehicle is not rented.");
            }

            activeRental.Return(returnDate);
            ActiveRentalId = null;

            return activeRental;
        }
    }
}
