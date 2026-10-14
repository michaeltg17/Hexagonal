using System;
using System.Linq;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ListAvailableVehicles
{
    /// <summary>
    /// Use case that lists the vehicles of the fleet.
    /// </summary>
    public sealed class ListAvailableVehiclesUseCase : IUseCase<ListAvailableVehiclesInput>
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IListAvailableVehiclesOutputPort _outputPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListAvailableVehiclesUseCase"/> class.
        /// </summary>
        /// <param name="vehicleRepository">Vehicle repository.</param>
        /// <param name="outputPort">Output port.</param>
        public ListAvailableVehiclesUseCase(IVehicleRepository vehicleRepository, IListAvailableVehiclesOutputPort outputPort)
        {
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
            _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));
        }

        /// <summary>
        /// Executes the use case.
        /// </summary>
        /// <param name="input">Input message.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Execute(ListAvailableVehiclesInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var vehicles = await _vehicleRepository.GetAll();

            var result = vehicles
                .Where(vehicle => input.IsAvailable == false ? !vehicle.IsAvailable : vehicle.IsAvailable)
                .Select(VehicleDtoMapper.Map)
                .ToList();

            _outputPort.StandardHandle(new ListAvailableVehiclesOutput(result));
        }
    }
}
