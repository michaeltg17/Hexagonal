namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetVehicle
{
    /// <summary>
    /// Output message of the get vehicle use case.
    /// </summary>
    public sealed class GetVehicleOutput : IUseCaseOutput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetVehicleOutput"/> class.
        /// </summary>
        /// <param name="vehicle">The vehicle.</param>
        public GetVehicleOutput(VehicleDto vehicle)
        {
            Vehicle = vehicle;
        }

        /// <summary>
        /// Gets the vehicle.
        /// </summary>
        public VehicleDto Vehicle { get; }
    }
}
