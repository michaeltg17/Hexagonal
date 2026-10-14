using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.ValueObjects;

namespace GtMotive.Estimate.Microservice.Domain.Interfaces
{
    /// <summary>
    /// Factory contract for rentals.
    /// </summary>
    public interface IRentalFactory
    {
        /// <summary>
        /// Creates a new rental.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <param name="person">Person that rents the vehicle.</param>
        /// <returns>The new rental.</returns>
        IRental NewRental(VehicleId vehicleId, Person person);
    }
}
