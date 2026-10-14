using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ListAvailableVehicles;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.ListAvailableVehicles
{
    /// <summary>
    /// MediatR handler of the list available vehicles use case.
    /// </summary>
    public sealed class ListAvailableVehiclesHandler : IRequestHandler<ListAvailableVehiclesRequest, IWebApiPresenter>
    {
        private readonly IUseCase<ListAvailableVehiclesInput> _listAvailableVehiclesUseCase;
        private readonly ListAvailableVehiclesPresenter _presenter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListAvailableVehiclesHandler"/> class.
        /// </summary>
        /// <param name="listAvailableVehiclesUseCase">List available vehicles use case.</param>
        /// <param name="presenter">Web API presenter.</param>
        public ListAvailableVehiclesHandler(IUseCase<ListAvailableVehiclesInput> listAvailableVehiclesUseCase, ListAvailableVehiclesPresenter presenter)
        {
            _listAvailableVehiclesUseCase = listAvailableVehiclesUseCase ?? throw new ArgumentNullException(nameof(listAvailableVehiclesUseCase));
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
        }

        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The presenter with the action result.</returns>
        public async Task<IWebApiPresenter> Handle(ListAvailableVehiclesRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            _ = cancellationToken;

            await _listAvailableVehiclesUseCase.Execute(new ListAvailableVehiclesInput(request.IsAvailable));

            return _presenter;
        }
    }
}
