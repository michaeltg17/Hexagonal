using GtMotive.Estimate.Microservice.Domain.Vehicles.ValueObjects;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles.Interfaces
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
        /// <param name="client">Client that rents the vehicle.</param>
        /// <returns>The new rental.</returns>
        IRental NewRental(VehicleId vehicleId, Client client);
    }
}
