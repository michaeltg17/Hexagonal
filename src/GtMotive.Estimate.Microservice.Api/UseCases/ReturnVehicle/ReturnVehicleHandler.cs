using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle
{
    /// <summary>
    /// MediatR handler of the return vehicle use case.
    /// </summary>
    public sealed class ReturnVehicleHandler : IRequestHandler<ReturnVehicleRequest, IWebApiPresenter>
    {
        private readonly IUseCase<ReturnVehicleInput> _returnVehicleUseCase;
        private readonly ReturnVehiclePresenter _presenter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnVehicleHandler"/> class.
        /// </summary>
        /// <param name="returnVehicleUseCase">Return vehicle use case.</param>
        /// <param name="presenter">Web API presenter.</param>
        public ReturnVehicleHandler(IUseCase<ReturnVehicleInput> returnVehicleUseCase, ReturnVehiclePresenter presenter)
        {
            _returnVehicleUseCase = returnVehicleUseCase ?? throw new ArgumentNullException(nameof(returnVehicleUseCase));
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
        }

        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The presenter with the action result.</returns>
        public async Task<IWebApiPresenter> Handle(ReturnVehicleRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            _ = cancellationToken;

            await _returnVehicleUseCase.Execute(new ReturnVehicleInput(request.VehicleId));

            return _presenter;
        }
    }
}
