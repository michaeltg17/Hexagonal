using System;
using GtMotive.Estimate.Microservice.Api.ViewModels;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.RentVehicle
{
    /// <summary>
    /// MediatR request of the rent vehicle use case.
    /// </summary>
    public sealed class RentVehicleRequest : IRequest<IWebApiPresenter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RentVehicleRequest"/> class.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <param name="vehicle">Rental data.</param>
        public RentVehicleRequest(Guid vehicleId, RentVehicleViewModel vehicle)
        {
            VehicleId = vehicleId;
            Vehicle = vehicle;
        }

        /// <summary>
        /// Gets the vehicle identifier.
        /// </summary>
        public Guid VehicleId { get; }

        /// <summary>
        /// Gets the rental data.
        /// </summary>
        public RentVehicleViewModel Vehicle { get; }
    }
}
