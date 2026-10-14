namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle
{
    /// <summary>
    /// Output port of the return vehicle use case.
    /// </summary>
    public interface IReturnVehicleOutputPort : IOutputPortStandard<ReturnVehicleOutput>, IOutputPortNotFound
    {
        /// <summary>
        /// Informs that the vehicle is not rented.
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
