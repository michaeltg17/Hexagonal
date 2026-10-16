using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles.ValueObjects;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle
{
    /// <summary>
    /// Use case that rents a vehicle to a person.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="RentVehicleUseCase"/> class.
    /// </remarks>
    /// <param name="vehicleRepository">Vehicle repository.</param>
    /// <param name="vehicleService">Vehicle domain service.</param>
    /// <param name="unitOfWork">Unit of work.</param>
    /// <param name="outputPort">Output port.</param>
    public sealed class RentVehicleUseCase(
        IVehicleRepository vehicleRepository,
        VehicleService vehicleService,
        IUnitOfWork unitOfWork,
        IRentVehicleOutputPort outputPort) : IUseCase<RentVehicleInput>
    {
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        private readonly VehicleService _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly IRentVehicleOutputPort _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));

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

                await _unitOfWork.Save();

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
