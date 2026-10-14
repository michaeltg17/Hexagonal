using System;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.GetVehicle
{
    /// <summary>
    /// MediatR request of the get vehicle use case.
    /// </summary>
    public sealed class GetVehicleRequest : IRequest<IWebApiPresenter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetVehicleRequest"/> class.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        public GetVehicleRequest(Guid vehicleId)
        {
            VehicleId = vehicleId;
        }

        /// <summary>
        /// Gets the vehicle identifier.
        /// </summary>
        public Guid VehicleId { get; }
    }
}
