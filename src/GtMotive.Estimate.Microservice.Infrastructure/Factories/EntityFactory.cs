using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.ValueObjects;
using GtMotive.Estimate.Microservice.Infrastructure.Entities;

namespace GtMotive.Estimate.Microservice.Infrastructure.Factories
{
    /// <summary>
    /// Factory of the concrete entities.
    /// </summary>
    public sealed class EntityFactory : IVehicleFactory, IRentalFactory
    {
        /// <summary>
        /// Creates a new vehicle.
        /// </summary>
        /// <param name="plate">Plate.</param>
        /// <param name="make">Make.</param>
        /// <param name="model">Model.</param>
        /// <param name="manufactureDate">Manufacture date.</param>
        /// <returns>The new vehicle.</returns>
        public IVehicle NewVehicle(Plate plate, Make make, Model model, ManufactureDate manufactureDate)
        {
            return new VehicleEntity(plate, make, model, manufactureDate);
        }

        /// <summary>
        /// Creates a new rental.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <param name="person">Person that rents the vehicle.</param>
        /// <returns>The new rental.</returns>
        public IRental NewRental(VehicleId vehicleId, Person person)
        {
            return new RentalEntity(vehicleId, person);
        }
    }
}
