using System;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle
{
    /// <summary>
    /// Input message of the return vehicle use case.
    /// </summary>
    public sealed class ReturnVehicleInput : IUseCaseInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnVehicleInput"/> class.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        public ReturnVehicleInput(Guid vehicleId)
        {
            VehicleId = vehicleId;
        }

        /// <summary>
        /// Gets the vehicle identifier.
        /// </summary>
        public Guid VehicleId { get; }
    }
}
