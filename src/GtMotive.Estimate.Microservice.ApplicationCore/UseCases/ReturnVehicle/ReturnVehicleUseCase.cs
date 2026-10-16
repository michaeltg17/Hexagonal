using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles.ValueObjects;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle
{
    /// <summary>
    /// Use case that returns a rented vehicle.
    /// </summary>
    public sealed class ReturnVehicleUseCase : IUseCase<ReturnVehicleInput>
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly VehicleService _vehicleService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReturnVehicleOutputPort _outputPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnVehicleUseCase"/> class.
        /// </summary>
        /// <param name="vehicleRepository">Vehicle repository.</param>
        /// <param name="vehicleService">Vehicle domain service.</param>
        /// <param name="unitOfWork">Unit of work.</param>
        /// <param name="outputPort">Output port.</param>
        public ReturnVehicleUseCase(
            IVehicleRepository vehicleRepository,
            VehicleService vehicleService,
            IUnitOfWork unitOfWork,
            IReturnVehicleOutputPort outputPort)
        {
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
            _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));
        }

        /// <summary>
        /// Executes the use case.
        /// </summary>
        /// <param name="input">Input message.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Execute(ReturnVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            try
            {
                var vehicle = await _vehicleRepository.GetBy(new VehicleId(input.VehicleId));

                if (vehicle is null)
                {
                    _outputPort.NotFoundHandle($"The vehicle with id '{input.VehicleId}' was not found.");
                    return;
                }

                await _vehicleService.Return(vehicle);

                await _unitOfWork.Save();

                _outputPort.StandardHandle(new ReturnVehicleOutput(VehicleDtoMapper.Map(vehicle)));
            }
            catch (VehicleNotRentedException ex)
            {
                _outputPort.ConflictHandle(ex.Message);
            }
            catch (RentalAlreadyReturnedException ex)
            {
                _outputPort.ConflictHandle(ex.Message);
            }
            catch (VehicleConcurrencyException ex)
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
