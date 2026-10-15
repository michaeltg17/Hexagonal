using System;
using GtMotive.Estimate.Microservice.Domain.ValueObjects;

namespace GtMotive.Estimate.Microservice.Domain.Interfaces
{
    /// <summary>
    /// Rental contract.
    /// </summary>
    public interface IRental
    {
        /// <summary>
        /// Gets the rental identifier.
        /// </summary>
        RentalId Id { get; }

        /// <summary>
        /// Gets the vehicle identifier.
        /// </summary>
        VehicleId VehicleId { get; }

        /// <summary>
        /// Gets the person that rents the vehicle.
        /// </summary>
        Person Person { get; }

        /// <summary>
        /// Gets the rent date.
        /// </summary>
        DateTime? RentDate { get; }

        /// <summary>
        /// Gets the return date.
        /// </summary>
        DateTime? ReturnDate { get; }

        /// <summary>
        /// Gets a value indicating whether the rental is active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Marks the rental as rented.
        /// </summary>
        /// <param name="rentDate">Rent date.</param>
        void Rent(DateTime rentDate);

        /// <summary>
        /// Marks the rental as returned.
        /// </summary>
        /// <param name="returnDate">Return date.</param>
        void Return(DateTime returnDate);
    }
}
