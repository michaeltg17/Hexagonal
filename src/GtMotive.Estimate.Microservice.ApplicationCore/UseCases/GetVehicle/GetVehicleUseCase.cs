using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetVehicle
{
    /// <summary>
    /// Use case that gets a vehicle of the fleet by its identifier.
    /// </summary>
    public sealed class GetVehicleUseCase : IUseCase<GetVehicleInput>
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IGetVehicleOutputPort _outputPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetVehicleUseCase"/> class.
        /// </summary>
        /// <param name="vehicleRepository">Vehicle repository.</param>
        /// <param name="outputPort">Output port.</param>
        public GetVehicleUseCase(IVehicleRepository vehicleRepository, IGetVehicleOutputPort outputPort)
        {
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
            _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));
        }

        /// <summary>
        /// Executes the use case.
        /// </summary>
        /// <param name="input">Input message.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Execute(GetVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var vehicle = await _vehicleRepository.GetBy(new VehicleId(input.VehicleId));

            if (vehicle is null)
            {
                _outputPort.NotFoundHandle($"The vehicle with id '{input.VehicleId}' was not found.");
                return;
            }

            _outputPort.StandardHandle(new GetVehicleOutput(VehicleDtoMapper.Map(vehicle)));
        }
    }
}
