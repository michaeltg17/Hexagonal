using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Exceptions;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Services;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle
{
    /// <summary>
    /// Use case that rents a vehicle to a person.
    /// </summary>
    public sealed class RentVehicleUseCase : IUseCase<RentVehicleInput>
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly VehicleService _vehicleService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRentVehicleOutputPort _outputPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="RentVehicleUseCase"/> class.
        /// </summary>
        /// <param name="vehicleRepository">Vehicle repository.</param>
        /// <param name="vehicleService">Vehicle domain service.</param>
        /// <param name="unitOfWork">Unit of work.</param>
        /// <param name="outputPort">Output port.</param>
        public RentVehicleUseCase(
            IVehicleRepository vehicleRepository,
            VehicleService vehicleService,
            IUnitOfWork unitOfWork,
            IRentVehicleOutputPort outputPort)
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
        public async Task Execute(RentVehicleInput input)
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

                await _vehicleService.Rent(vehicle, new Client(input.ClientEmail));

                await _unitOfWork.SaveInTransaction();

                _outputPort.StandardHandle(new RentVehicleOutput(VehicleDtoMapper.Map(vehicle)));
            }
            catch (VehicleAlreadyRentedException ex)
            {
                _outputPort.ConflictHandle(ex.Message);
            }
            catch (ClientAlreadyHasActiveRentalException ex)
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
