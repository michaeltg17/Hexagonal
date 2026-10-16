using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles.ValueObjects;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Domain service that orchestrates the vehicle aggregate behavior.
    /// </summary>
    public class VehicleService
    {
        private readonly IVehicleFactory _vehicleFactory;
        private readonly IRentalFactory _rentalFactory;
        private readonly IVehicleRepository _vehicleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleService"/> class.
        /// </summary>
        /// <param name="vehicleFactory">Vehicle factory.</param>
        /// <param name="rentalFactory">Rental factory.</param>
        /// <param name="vehicleRepository">Vehicle repository.</param>
        public VehicleService(
            IVehicleFactory vehicleFactory,
            IRentalFactory rentalFactory,
            IVehicleRepository vehicleRepository)
        {
            _vehicleFactory = vehicleFactory ?? throw new ArgumentNullException(nameof(vehicleFactory));
            _rentalFactory = rentalFactory ?? throw new ArgumentNullException(nameof(rentalFactory));
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        }

        /// <summary>
        /// Creates a new vehicle in the fleet.
        /// </summary>
        /// <param name="plate">Plate.</param>
        /// <param name="make">Make.</param>
        /// <param name="model">Model.</param>
        /// <param name="manufactureDate">Manufacture date.</param>
        /// <returns>The created vehicle.</returns>
        public async Task<IVehicle> Create(Plate plate, Make make, Model model, ManufactureDate manufactureDate)
        {
            if (await _vehicleRepository.ExistsByPlate(plate))
            {
                throw new VehicleAlreadyExistsException(
                    $"The vehicle with plate {plate} already exists in the fleet.");
            }

            var vehicle = _vehicleFactory.NewVehicle(plate, make, model, manufactureDate);
            await _vehicleRepository.Add(vehicle);

            return vehicle;
        }

        /// <summary>
        /// Rents a vehicle to a client.
        /// </summary>
        /// <param name="vehicle">Vehicle to rent.</param>
        /// <param name="client">Client that rents the vehicle.</param>
        /// <returns>The created rental.</returns>
        public async Task<IRental> Rent(IVehicle vehicle, Client client)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            if (await _vehicleRepository.ClientHasActiveRental(client))
            {
                throw new ClientAlreadyHasActiveRentalException("The client already has an active rental.");
            }

            var rental = vehicle.Rent(_rentalFactory, client, DateTime.UtcNow);
            await _vehicleRepository.Update(vehicle);
            await _vehicleRepository.AddRental(rental);

            return rental;
        }

        /// <summary>
        /// Returns the active rental of a vehicle.
        /// </summary>
        /// <param name="vehicle">Vehicle to return.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Return(IVehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            var rental = vehicle.Return(DateTime.UtcNow);
            await _vehicleRepository.Update(vehicle);
            await _vehicleRepository.UpdateRental(rental);
        }
    }
}
