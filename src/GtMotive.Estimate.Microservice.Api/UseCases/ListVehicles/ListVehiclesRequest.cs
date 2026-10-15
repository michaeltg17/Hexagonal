using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.ListVehicles
{
    /// <summary>
    /// MediatR request of the list vehicles use case.
    /// </summary>
    public sealed record ListVehiclesRequest : IRequest<IWebApiPresenter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListVehiclesRequest"/> class.
        /// </summary>
        /// <param name="available">Availability filter: <c>null</c> all, <c>true</c> only available, <c>false</c> only unavailable.</param>
        public ListVehiclesRequest(bool? available)
        {
            Available = available;
        }

        /// <summary>
        /// Gets the availability filter.
        /// </summary>
        public bool? Available { get; init; }
    }
}
