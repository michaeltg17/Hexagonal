namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetVehicle
{
    /// <summary>
    /// Output port of the get vehicle use case.
    /// </summary>
    public interface IGetVehicleOutputPort : IOutputPortStandard<GetVehicleOutput>, IOutputPortNotFound
    {
        /// <summary>
        /// Informs that the input was invalid.
        /// </summary>
        /// <param name="message">Text description.</param>
        void BadRequestHandle(string message);
    }
}
