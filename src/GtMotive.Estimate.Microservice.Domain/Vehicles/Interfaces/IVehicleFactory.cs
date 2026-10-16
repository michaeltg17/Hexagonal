using GtMotive.Estimate.Microservice.Domain.Vehicles.ValueObjects;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles.Interfaces
{
    /// <summary>
    /// Factory contract for vehicles.
    /// </summary>
    public interface IVehicleFactory
    {
        /// <summary>
        /// Creates a new vehicle.
        /// </summary>
        /// <param name="plate">Plate.</param>
        /// <param name="make">Make.</param>
        /// <param name="model">Model.</param>
        /// <param name="manufactureDate">Manufacture date.</param>
        /// <returns>The new vehicle.</returns>
        IVehicle NewVehicle(Plate plate, Make make, Model model, ManufactureDate manufactureDate);
    }
}
