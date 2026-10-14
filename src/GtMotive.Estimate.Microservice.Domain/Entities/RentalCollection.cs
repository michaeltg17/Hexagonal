using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GtMotive.Estimate.Microservice.Domain.Entities
{
    /// <summary>
    /// First-class collection of rentals of a vehicle.
    /// </summary>
    public sealed class RentalCollection
    {
        private readonly List<IRental> _rentals = [];

        /// <summary>
        /// Adds a rental to the collection.
        /// </summary>
        /// <param name="rental">Rental to add.</param>
        public void Add(IRental rental)
        {
            ArgumentNullException.ThrowIfNull(rental);

            _rentals.Add(rental);
        }

        /// <summary>
        /// Gets all the rentals of the collection.
        /// </summary>
        /// <returns>The rentals.</returns>
        public IReadOnlyCollection<IRental> GetRentals()
        {
            return new ReadOnlyCollection<IRental>(_rentals);
        }

        /// <summary>
        /// Gets the active rental of the collection.
        /// </summary>
        /// <returns>The active rental or null if there is not any.</returns>
        public IRental GetActive()
        {
            return _rentals.FirstOrDefault(rental => rental.IsActive);
        }

        /// <summary>
        /// Gets a value indicating whether the collection has an active rental.
        /// </summary>
        /// <returns>Whether the collection has an active rental.</returns>
        public bool HasActiveRental()
        {
            return this.GetActive() is not null;
        }
    }
}
