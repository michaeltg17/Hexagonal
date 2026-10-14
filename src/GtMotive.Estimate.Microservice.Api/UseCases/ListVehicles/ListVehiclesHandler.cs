using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ListVehicles;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.ListVehicles
{
    /// <summary>
    /// MediatR handler of the list vehicles use case.
    /// </summary>
    public sealed class ListVehiclesHandler : IRequestHandler<ListVehiclesRequest, IWebApiPresenter>
    {
        private readonly IUseCase<ListVehiclesInput> _listVehiclesUseCase;
        private readonly ListVehiclesPresenter _presenter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListVehiclesHandler"/> class.
        /// </summary>
        /// <param name="listVehiclesUseCase">List vehicles use case.</param>
        /// <param name="presenter">Web API presenter.</param>
        public ListVehiclesHandler(IUseCase<ListVehiclesInput> listVehiclesUseCase, ListVehiclesPresenter presenter)
        {
            _listVehiclesUseCase = listVehiclesUseCase ?? throw new ArgumentNullException(nameof(listVehiclesUseCase));
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
        }

        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The presenter with the action result.</returns>
        public async Task<IWebApiPresenter> Handle(ListVehiclesRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            _ = cancellationToken;

            await _listVehiclesUseCase.Execute(new ListVehiclesInput(request.Available));

            return _presenter;
        }
    }
}
