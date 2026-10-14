namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle
{
    /// <summary>
    /// Output message of the return vehicle use case.
    /// </summary>
    public sealed class ReturnVehicleOutput : IUseCaseOutput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnVehicleOutput"/> class.
        /// </summary>
        /// <param name="vehicle">The returned vehicle.</param>
        public ReturnVehicleOutput(VehicleDto vehicle)
        {
            Vehicle = vehicle;
        }

        /// <summary>
        /// Gets the returned vehicle.
        /// </summary>
        public VehicleDto Vehicle { get; }
    }
}
