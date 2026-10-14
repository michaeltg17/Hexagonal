namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ListAvailableVehicles
{
    /// <summary>
    /// Input message of the list available vehicles use case.
    /// </summary>
    public sealed class ListAvailableVehiclesInput : IUseCaseInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListAvailableVehiclesInput"/> class.
        /// </summary>
        /// <param name="isAvailable">Whether to list only the available vehicles.</param>
        public ListAvailableVehiclesInput(bool? isAvailable)
        {
            IsAvailable = isAvailable;
        }

        /// <summary>
        /// Gets a value indicating whether to list only the available vehicles.
        /// </summary>
        public bool? IsAvailable { get; }
    }
}
