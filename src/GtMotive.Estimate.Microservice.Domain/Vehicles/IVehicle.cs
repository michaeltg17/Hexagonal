using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Vehicle aggregate root contract.
    /// </summary>
    public interface IVehicle
    {
        /// <summary>
        /// Gets the vehicle identifier.
        /// </summary>
        VehicleId Id { get; }

        /// <summary>
        /// Gets the plate.
        /// </summary>
        Plate Plate { get; }

        /// <summary>
        /// Gets the make.
        /// </summary>
        Make Make { get; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        Model Model { get; }

        /// <summary>
        /// Gets the manufacture date.
        /// </summary>
        ManufactureDate ManufactureDate { get; }

        /// <summary>
        /// Gets the rentals of the vehicle.
        /// </summary>
        RentalCollection Rentals { get; }

        /// <summary>
        /// Gets a value indicating whether the vehicle is available.
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// Rents the vehicle to a person.
        /// </summary>
        /// <param name="rentalFactory">Rental factory.</param>
        /// <param name="person">Person that rents the vehicle.</param>
        /// <param name="rentDate">Rent date.</param>
        /// <returns>The created rental.</returns>
        IRental Rent(IRentalFactory rentalFactory, Person person, DateTime rentDate);

        /// <summary>
        /// Returns the active rental of the vehicle.
        /// </summary>
        /// <param name="returnDate">Return date.</param>
        /// <returns>The returned rental.</returns>
        IRental Return(DateTime returnDate);
    }
}
