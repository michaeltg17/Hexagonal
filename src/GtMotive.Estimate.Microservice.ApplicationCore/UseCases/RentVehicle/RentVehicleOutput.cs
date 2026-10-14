namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle
{
    /// <summary>
    /// Output message of the rent vehicle use case.
    /// </summary>
    public sealed class RentVehicleOutput : IUseCaseOutput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RentVehicleOutput"/> class.
        /// </summary>
        /// <param name="vehicle">The rented vehicle.</param>
        public RentVehicleOutput(VehicleDto vehicle)
        {
            Vehicle = vehicle;
        }

        /// <summary>
        /// Gets the rented vehicle.
        /// </summary>
        public VehicleDto Vehicle { get; }
    }
}
