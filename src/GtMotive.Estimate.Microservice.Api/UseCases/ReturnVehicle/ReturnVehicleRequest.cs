using System;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle
{
    /// <summary>
    /// MediatR request of the return vehicle use case.
    /// </summary>
    public sealed class ReturnVehicleRequest : IRequest<IWebApiPresenter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnVehicleRequest"/> class.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        public ReturnVehicleRequest(Guid vehicleId)
        {
            VehicleId = vehicleId;
        }

        /// <summary>
        /// Gets the vehicle identifier.
        /// </summary>
        public Guid VehicleId { get; }
    }
}
