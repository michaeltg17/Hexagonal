using GtMotive.Estimate.Microservice.Api.ViewModels;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.CreateVehicle
{
    /// <summary>
    /// MediatR request of the create vehicle use case.
    /// </summary>
    public sealed class CreateVehicleRequest : IRequest<IWebApiPresenter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateVehicleRequest"/> class.
        /// </summary>
        /// <param name="vehicle">Vehicle to create.</param>
        public CreateVehicleRequest(CreateVehicleViewModel vehicle)
        {
            Vehicle = vehicle;
        }

        /// <summary>
        /// Gets the vehicle to create.
        /// </summary>
        public CreateVehicleViewModel Vehicle { get; }
    }
}
