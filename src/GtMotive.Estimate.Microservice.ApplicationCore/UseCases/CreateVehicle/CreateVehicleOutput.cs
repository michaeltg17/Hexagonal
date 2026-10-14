namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle
{
    /// <summary>
    /// Output message of the create vehicle use case.
    /// </summary>
    public sealed class CreateVehicleOutput : IUseCaseOutput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateVehicleOutput"/> class.
        /// </summary>
        /// <param name="vehicle">The created vehicle.</param>
        public CreateVehicleOutput(VehicleDto vehicle)
        {
            Vehicle = vehicle;
        }

        /// <summary>
        /// Gets the created vehicle.
        /// </summary>
        public VehicleDto Vehicle { get; }
    }
}
