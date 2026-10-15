using System;
using GtMotive.Estimate.Microservice.Api.ViewModels;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.RentVehicle
{
    /// <summary>
    /// MediatR request of the rent vehicle use case.
    /// </summary>
    public sealed record RentVehicleRequest : IRequest<IWebApiPresenter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RentVehicleRequest"/> class.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <param name="rental">Rental data.</param>
        public RentVehicleRequest(Guid vehicleId, RentVehicleViewModel rental)
        {
            VehicleId = vehicleId;
            Rental = rental;
        }

        /// <summary>
        /// Gets the vehicle identifier.
        /// </summary>
        public Guid VehicleId { get; init; }

        /// <summary>
        /// Gets the rental data.
        /// </summary>
        public RentVehicleViewModel Rental { get; init; }
    }
}
