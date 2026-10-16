using System;
using System.Linq;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Interfaces;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ListVehicles
{
    /// <summary>
    /// Use case that lists the vehicles of the fleet.
    /// </summary>
    public sealed class ListVehiclesUseCase : IUseCase<ListVehiclesInput>
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IListVehiclesOutputPort _outputPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListVehiclesUseCase"/> class.
        /// </summary>
        /// <param name="vehicleRepository">Vehicle repository.</param>
        /// <param name="outputPort">Output port.</param>
        public ListVehiclesUseCase(IVehicleRepository vehicleRepository, IListVehiclesOutputPort outputPort)
        {
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
            _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));
        }

        /// <summary>
        /// Executes the use case.
        /// </summary>
        /// <param name="input">Input message.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Execute(ListVehiclesInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var vehicles = await _vehicleRepository.GetAll(input.Available);

            var result = vehicles
                .Select(VehicleDtoMapper.Map)
                .ToList();

            _outputPort.StandardHandle(new ListVehiclesOutput(result));
        }
    }
}
