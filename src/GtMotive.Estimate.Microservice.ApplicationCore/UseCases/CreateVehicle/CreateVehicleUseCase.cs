using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Exceptions;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Services;
using GtMotive.Estimate.Microservice.Domain.ValueObjects;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle
{
    /// <summary>
    /// Use case that creates a new vehicle in the fleet.
    /// </summary>
    public sealed class CreateVehicleUseCase : IUseCase<CreateVehicleInput>
    {
        private readonly VehicleService _vehicleService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICreateVehicleOutputPort _outputPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateVehicleUseCase"/> class.
        /// </summary>
        /// <param name="vehicleService">Vehicle domain service.</param>
        /// <param name="unitOfWork">Unit of work.</param>
        /// <param name="outputPort">Output port.</param>
        public CreateVehicleUseCase(VehicleService vehicleService, IUnitOfWork unitOfWork, ICreateVehicleOutputPort outputPort)
        {
            _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));
        }

        /// <summary>
        /// Executes the use case.
        /// </summary>
        /// <param name="input">Input message.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Execute(CreateVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            try
            {
                var vehicle = await _vehicleService.Create(
                    new Plate(input.Plate),
                    new Make(input.Make),
                    new Model(input.Model),
                    new ManufactureDate(input.ManufactureDate));

                await _unitOfWork.Save();

                _outputPort.StandardHandle(new CreateVehicleOutput(VehicleDtoMapper.Map(vehicle)));
            }
            catch (VehicleAlreadyExistsException ex)
            {
                _outputPort.ConflictHandle(ex.Message);
            }
            catch (DomainException ex)
            {
                _outputPort.BadRequestHandle(ex.Message);
            }
        }
    }
}
