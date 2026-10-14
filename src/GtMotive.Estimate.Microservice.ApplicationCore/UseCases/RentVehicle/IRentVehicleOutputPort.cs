namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle
{
    /// <summary>
    /// Output port of the rent vehicle use case.
    /// </summary>
    public interface IRentVehicleOutputPort : IOutputPortStandard<RentVehicleOutput>, IOutputPortNotFound
    {
        /// <summary>
        /// Informs that the rental can not be created because of a conflict.
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
