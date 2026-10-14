using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.ValueObjects;
using GtMotive.Estimate.Microservice.Infrastructure.Entities;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Entities;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure.Repositories
{
    /// <summary>
    /// Mongo implementation of the vehicle repository.
    /// </summary>
    public sealed class VehicleRepository : IVehicleRepository
    {
        private const string CollectionName = "vehicles";

        private readonly IMongoCollection<VehicleDocument> _collection;
        private readonly List<IVehicle> _toSave = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleRepository"/> class.
        /// </summary>
        /// <param name="database">Mongo database.</param>
        public VehicleRepository(IMongoDatabase database)
        {
            ArgumentNullException.ThrowIfNull(database);

            _collection = database.GetCollection<VehicleDocument>(CollectionName);
        }

        /// <summary>
        /// Adds a new vehicle to the fleet.
        /// </summary>
        /// <param name="vehicle">Vehicle to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task Add(IVehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            _toSave.Add(vehicle);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets a vehicle by its identifier.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <returns>The vehicle.</returns>
        public async Task<IVehicle> GetBy(VehicleId vehicleId)
        {
            var document = await _collection
                .Find(vehicle => vehicle.Id == vehicleId.ToGuid())
                .FirstOrDefaultAsync();

            return document is null ? null : MapToEntity(document);
        }

        /// <summary>
        /// Gets the vehicles of the fleet, optionally filtered by availability.
        /// </summary>
        /// <param name="available">Availability filter: <c>true</c> only available, <c>false</c> only unavailable, <c>null</c> all.</param>
        /// <returns>The vehicles of the fleet.</returns>
        public async Task<IReadOnlyCollection<IVehicle>> GetAll(bool? available)
        {
            var documents = available switch
            {
                true => await _collection
                    .Find(vehicle => !vehicle.Rentals.Any(rental => rental.ReturnDate == null))
                    .ToListAsync(),
                false => await _collection
                    .Find(vehicle => vehicle.Rentals.Any(rental => rental.ReturnDate == null))
                    .ToListAsync(),
                _ => await _collection
                    .Find(FilterDefinition<VehicleDocument>.Empty)
                    .ToListAsync(),
            };

            return documents.Select(MapToEntity).ToList();
        }

        /// <summary>
        /// Updates an existing vehicle.
        /// </summary>
        /// <param name="vehicle">Vehicle to update.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task Update(IVehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            _toSave.Add(vehicle);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets a value indicating whether a person has an active rental in the fleet.
        /// </summary>
        /// <param name="person">Person to check.</param>
        /// <returns>Whether the person has an active rental.</returns>
        public async Task<bool> PersonHasActiveRental(Person person)
        {
            var count = await _collection.CountDocumentsAsync(
                vehicle => vehicle.Rentals.Any(rental => rental.Person == person.Value && rental.ReturnDate == null));

            return count > 0;
        }

        /// <summary>
        /// Gets a value indicating whether a vehicle with the plate already exists in the fleet.
        /// </summary>
        /// <param name="plate">Plate to check.</param>
        /// <returns>Whether the plate already exists in the fleet.</returns>
        public async Task<bool> ExistsByPlate(Plate plate)
        {
            var count = await _collection.CountDocumentsAsync(vehicle => vehicle.Plate == plate.Value);

            return count > 0;
        }

        /// <summary>
        /// Persists all the pending changes.
        /// </summary>
        /// <returns>The number of persisted changes.</returns>
        internal async Task<int> SaveChanges()
        {
            var saved = 0;

            foreach (var vehicle in _toSave)
            {
                var document = MapToDocument(vehicle);
                await _collection.ReplaceOneAsync(
                    v => v.Id == document.Id,
                    document,
                    new ReplaceOptions { IsUpsert = true });
                saved++;
            }

            _toSave.Clear();

            return saved;
        }

        private static VehicleDocument MapToDocument(IVehicle vehicle)
        {
            return new VehicleDocument
            {
                Id = vehicle.Id.ToGuid(),
                Plate = vehicle.Plate.Value,
                Make = vehicle.Make.Value,
                Model = vehicle.Model.Value,
                ManufactureDate = vehicle.ManufactureDate.Value,
                Rentals = [.. vehicle.Rentals.GetRentals().Select(rental => new RentalDocument
                {
                    Id = rental.Id.ToGuid(),
                    VehicleId = rental.VehicleId.ToGuid(),
                    Person = rental.Person.Value,
                    RentDate = rental.RentDate,
                    ReturnDate = rental.ReturnDate,
                })],
            };
        }

        private static VehicleEntity MapToEntity(VehicleDocument document)
        {
            var rentals = document.Rentals
                .Select(rental => new RentalEntity(
                    new RentalId(rental.Id),
                    new VehicleId(rental.VehicleId),
                    new Person(rental.Person),
                    rental.RentDate,
                    rental.ReturnDate))
                .ToList();

            return new VehicleEntity(
                new VehicleId(document.Id),
                new Plate(document.Plate),
                new Make(document.Make),
                new Model(document.Model),
                new ManufactureDate(document.ManufactureDate),
                rentals);
        }
    }
}
