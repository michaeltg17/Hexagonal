namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ListVehicles
{
    /// <summary>
    /// Input message of the list vehicles use case.
    /// </summary>
    public sealed class ListVehiclesInput : IUseCaseInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListVehiclesInput"/> class.
        /// </summary>
        /// <param name="available">Availability filter: <c>null</c> all, <c>true</c> only available, <c>false</c> only unavailable.</param>
        public ListVehiclesInput(bool? available)
        {
            Available = available;
        }

        /// <summary>
        /// Gets the availability filter.
        /// </summary>
        public bool? Available { get; }
    }
}
