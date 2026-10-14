using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.ListAvailableVehicles
{
    /// <summary>
    /// MediatR request of the list available vehicles use case.
    /// </summary>
    public sealed class ListAvailableVehiclesRequest : IRequest<IWebApiPresenter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListAvailableVehiclesRequest"/> class.
        /// </summary>
        /// <param name="isAvailable">Whether to list only the available vehicles.</param>
        public ListAvailableVehiclesRequest(bool? isAvailable)
        {
            IsAvailable = isAvailable;
        }

        /// <summary>
        /// Gets a value indicating whether to list only the available vehicles.
        /// </summary>
        public bool? IsAvailable { get; }
    }
}
