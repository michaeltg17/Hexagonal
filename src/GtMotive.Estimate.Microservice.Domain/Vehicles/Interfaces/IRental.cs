using System;
using GtMotive.Estimate.Microservice.Domain.Vehicles.ValueObjects;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles.Interfaces
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
        /// Gets the client that rents the vehicle.
        /// </summary>
        Client Client { get; }

        /// <summary>
        /// Gets the start date.
        /// </summary>
        DateTime StartDate { get; }

        /// <summary>
        /// Gets the end date.
        /// </summary>
        DateTime? EndDate { get; }

        /// <summary>
        /// Gets a value indicating whether the rental is active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Marks the rental as ended.
        /// </summary>
        /// <param name="endDate">End date.</param>
        void End(DateTime endDate);
    }
}
