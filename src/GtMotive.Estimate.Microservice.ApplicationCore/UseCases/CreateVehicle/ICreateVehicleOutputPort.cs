namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle
{
    /// <summary>
    /// Output port of the create vehicle use case.
    /// </summary>
    public interface ICreateVehicleOutputPort : IOutputPortStandard<CreateVehicleOutput>
    {
        /// <summary>
        /// Informs that a vehicle with the same plate already exists.
        /// </summary>
        /// <param name="message">Text description.</param>
        void ConflictHandle(string message);

        /// <summary>
        /// Informs that the input was invalid.
        /// </summary>
        /// <param name="message">Text description.</param>
        void BadRequestHandle(string message);
    }
}
