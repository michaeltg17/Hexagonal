using System.Collections.Generic;
using System.Threading.Tasks;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Repository contract for the vehicle aggregate.
    /// </summary>
    public interface IVehicleRepository
    {
        /// <summary>
        /// Adds a new vehicle to the fleet.
        /// </summary>
        /// <param name="vehicle">Vehicle to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Add(IVehicle vehicle);

        /// <summary>
        /// Gets a vehicle by its identifier.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <returns>The vehicle.</returns>
        Task<IVehicle> GetBy(VehicleId vehicleId);

        /// <summary>
        /// Gets the vehicles of the fleet, optionally filtered by availability.
        /// </summary>
        /// <param name="available">Availability filter: <c>true</c> only available, <c>false</c> only unavailable, <c>null</c> all.</param>
        /// <returns>The vehicles of the fleet.</returns>
        Task<IReadOnlyCollection<IVehicle>> GetAll(bool? available);

        /// <summary>
        /// Updates an existing vehicle.
        /// </summary>
        /// <param name="vehicle">Vehicle to update.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Update(IVehicle vehicle);

        /// <summary>
        /// Queues a new rental for persistence.
        /// </summary>
        /// <param name="rental">Rental to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddRental(IRental rental);

        /// <summary>
        /// Queues an updated rental for persistence.
        /// </summary>
        /// <param name="rental">Rental to update.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateRental(IRental rental);

        /// <summary>
        /// Gets a value indicating whether a person has an active rental in the fleet.
        /// </summary>
        /// <param name="person">Person to check.</param>
        /// <returns>Whether the person has an active rental.</returns>
        Task<bool> PersonHasActiveRental(Person person);

        /// <summary>
        /// Gets a value indicating whether a vehicle with the plate already exists in the fleet.
        /// </summary>
        /// <param name="plate">Plate to check.</param>
        /// <returns>Whether the plate already exists in the fleet.</returns>
        Task<bool> ExistsByPlate(Plate plate);
    }
}
