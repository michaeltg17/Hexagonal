using System.Collections.Generic;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles.ValueObjects;

namespace GtMotive.Estimate.Microservice.Infrastructure.Entities
{
    /// <summary>
    /// Concrete vehicle entity.
    /// </summary>
    public sealed class VehicleEntity : Vehicle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleEntity"/> class.
        /// </summary>
        public VehicleEntity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleEntity"/> class.
        /// </summary>
        /// <param name="plate">Plate.</param>
        /// <param name="make">Make.</param>
        /// <param name="model">Model.</param>
        /// <param name="manufactureDate">Manufacture date.</param>
        public VehicleEntity(Plate plate, Make make, Model model, ManufactureDate manufactureDate)
            : base(plate, make, model, manufactureDate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleEntity"/> class from persisted data.
        /// </summary>
        /// <param name="id">Vehicle identifier.</param>
        /// <param name="plate">Plate.</param>
        /// <param name="make">Make.</param>
        /// <param name="model">Model.</param>
        /// <param name="manufactureDate">Manufacture date.</param>
        /// <param name="rentals">Rentals of the vehicle.</param>
        internal VehicleEntity(
            VehicleId id,
            Plate plate,
            Make make,
            Model model,
            ManufactureDate manufactureDate,
            IEnumerable<IRental> rentals)
            : base(plate, make, model, manufactureDate)
        {
            Id = id;

            foreach (var rental in rentals)
            {
                Rentals.Add(rental);
            }
        }
    }
}
