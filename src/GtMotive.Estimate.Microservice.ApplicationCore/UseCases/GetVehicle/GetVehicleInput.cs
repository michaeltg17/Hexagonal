using System;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetVehicle
{
    /// <summary>
    /// Input message of the get vehicle use case.
    /// </summary>
    public sealed record GetVehicleInput : IUseCaseInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetVehicleInput"/> class.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        public GetVehicleInput(Guid vehicleId)
        {
            VehicleId = vehicleId;
        }

        /// <summary>
        /// Gets the vehicle identifier.
        /// </summary>
        public Guid VehicleId { get; init; }
    }
}
