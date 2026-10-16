using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetVehicle;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.GetVehicle
{
    /// <summary>
    /// MediatR handler of the get vehicle use case.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="GetVehicleHandler"/> class.
    /// </remarks>
    /// <param name="getVehicleUseCase">Get vehicle use case.</param>
    /// <param name="presenter">Web API presenter.</param>
    public sealed class GetVehicleHandler(IUseCase<GetVehicleInput> getVehicleUseCase, GetVehiclePresenter presenter) : IRequestHandler<GetVehicleRequest, IWebApiPresenter>
    {
        private readonly IUseCase<GetVehicleInput> _getVehicleUseCase = getVehicleUseCase ?? throw new ArgumentNullException(nameof(getVehicleUseCase));
        private readonly GetVehiclePresenter _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));

        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The presenter with the action result.</returns>
        public async Task<IWebApiPresenter> Handle(GetVehicleRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            _ = cancellationToken;

            await _getVehicleUseCase.Execute(new GetVehicleInput(request.VehicleId));

            return _presenter;
        }
    }
}
