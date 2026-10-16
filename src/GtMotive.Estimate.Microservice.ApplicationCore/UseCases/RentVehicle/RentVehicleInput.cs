using System;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle
{
    /// <summary>
    /// Input message of the rent vehicle use case.
    /// </summary>
    public sealed record RentVehicleInput : IUseCaseInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RentVehicleInput"/> class.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <param name="clientEmail">E-mail that identifies the client that rents the vehicle.</param>
        public RentVehicleInput(Guid vehicleId, string clientEmail)
        {
            VehicleId = vehicleId;
            ClientEmail = clientEmail;
        }

        /// <summary>
        /// Gets the vehicle identifier.
        /// </summary>
        public Guid VehicleId { get; init; }

        /// <summary>
        /// Gets the e-mail that identifies the client that rents the vehicle.
        /// </summary>
        public string ClientEmail { get; init; }
    }
}
