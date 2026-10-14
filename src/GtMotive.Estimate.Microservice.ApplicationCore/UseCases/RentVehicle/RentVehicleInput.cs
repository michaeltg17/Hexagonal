using System;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle
{
    /// <summary>
    /// Input message of the rent vehicle use case.
    /// </summary>
    public sealed class RentVehicleInput : IUseCaseInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RentVehicleInput"/> class.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <param name="person">Person that rents the vehicle.</param>
        public RentVehicleInput(Guid vehicleId, string person)
        {
            VehicleId = vehicleId;
            Person = person;
        }

        /// <summary>
        /// Gets the vehicle identifier.
        /// </summary>
        public Guid VehicleId { get; }

        /// <summary>
        /// Gets the person that rents the vehicle.
        /// </summary>
        public string Person { get; }
    }
}
