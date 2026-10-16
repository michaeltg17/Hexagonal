using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.CreateVehicle
{
    /// <summary>
    /// MediatR handler of the create vehicle use case.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="CreateVehicleHandler"/> class.
    /// </remarks>
    /// <param name="createVehicleUseCase">Create vehicle use case.</param>
    /// <param name="presenter">Web API presenter.</param>
    public sealed class CreateVehicleHandler(IUseCase<CreateVehicleInput> createVehicleUseCase, CreateVehiclePresenter presenter) : IRequestHandler<CreateVehicleRequest, IWebApiPresenter>
    {
        private readonly IUseCase<CreateVehicleInput> _createVehicleUseCase = createVehicleUseCase ?? throw new ArgumentNullException(nameof(createVehicleUseCase));
        private readonly CreateVehiclePresenter _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));

        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The presenter with the action result.</returns>
        public async Task<IWebApiPresenter> Handle(CreateVehicleRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            _ = cancellationToken;

            await _createVehicleUseCase.Execute(new CreateVehicleInput(
                request.Vehicle.Plate,
                request.Vehicle.Make,
                request.Vehicle.Model,
                request.Vehicle.ManufactureDate));

            return _presenter;
        }
    }
}
