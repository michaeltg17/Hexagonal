using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.ValueObjects;

namespace GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure
{
    internal sealed class InMemoryVehicleRepository : IVehicleRepository
    {
        private readonly List<IVehicle> _vehicles = [];

        public Task Add(IVehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            _vehicles.Add(vehicle);

            return Task.CompletedTask;
        }

        public Task<IVehicle> GetBy(VehicleId vehicleId)
        {
            return Task.FromResult(_vehicles.FirstOrDefault(vehicle => vehicle.Id == vehicleId));
        }

        public Task<IReadOnlyCollection<IVehicle>> GetAll()
        {
            var vehicles = _vehicles.ToList();

            return Task.FromResult<IReadOnlyCollection<IVehicle>>(vehicles);
        }

        public Task Update(IVehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            return Task.CompletedTask;
        }

        public Task<bool> PersonHasActiveRental(Person person)
        {
            var hasActiveRental = _vehicles.Any(
                vehicle => vehicle.Rentals.GetActive() is { } activeRental && activeRental.Person == person);

            return Task.FromResult(hasActiveRental);
        }

        public Task<bool> ExistsByPlate(Plate plate)
        {
            var exists = _vehicles.Any(vehicle => vehicle.Plate == plate);

            return Task.FromResult(exists);
        }
    }
}
