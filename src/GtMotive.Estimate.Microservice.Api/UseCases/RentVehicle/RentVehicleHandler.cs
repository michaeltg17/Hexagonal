using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.RentVehicle
{
    /// <summary>
    /// MediatR handler of the rent vehicle use case.
    /// </summary>
    public sealed class RentVehicleHandler : IRequestHandler<RentVehicleRequest, IWebApiPresenter>
    {
        private readonly IUseCase<RentVehicleInput> _rentVehicleUseCase;
        private readonly RentVehiclePresenter _presenter;

        /// <summary>
        /// Initializes a new instance of the <see cref="RentVehicleHandler"/> class.
        /// </summary>
        /// <param name="rentVehicleUseCase">Rent vehicle use case.</param>
        /// <param name="presenter">Web API presenter.</param>
        public RentVehicleHandler(IUseCase<RentVehicleInput> rentVehicleUseCase, RentVehiclePresenter presenter)
        {
            _rentVehicleUseCase = rentVehicleUseCase ?? throw new ArgumentNullException(nameof(rentVehicleUseCase));
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
        }

        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The presenter with the action result.</returns>
        public async Task<IWebApiPresenter> Handle(RentVehicleRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            _ = cancellationToken;

            await _rentVehicleUseCase.Execute(new RentVehicleInput(request.VehicleId, request.Rental.ClientEmail));

            return _presenter;
        }
    }
}
