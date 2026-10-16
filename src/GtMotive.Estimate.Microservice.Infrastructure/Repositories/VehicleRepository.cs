using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Exceptions;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
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
        private const int DuplicateKeyErrorCode = 11000;
        private const string VehiclesCollectionName = "vehicles";
        private const string RentalsCollectionName = "rentals";

        private static readonly ReplaceOptions NoUpsertReplaceOptions = new() { IsUpsert = false };

        private static readonly HashSet<int> TransientTransactionErrorCodes =
        [
            26, // LockContention.
            112, // CastError.
            136, // CappedPositionLost.
            150, // MaxTimeMSExpired.
            189, // TransactionAborted.
            251, // ExceededTimeQuota.
            262, // UserNotFound.
            263, // PrimarySteppedDown.
            11602, // StaleEpoch.
            43494, // StaleDbVersion.
        ];

        private readonly IMongoCollection<VehicleDocument> _vehicles;
        private readonly IMongoCollection<RentalDocument> _rentals;
        private readonly ConcurrentDictionary<Guid, long> _loadedVersions = new();
        private readonly List<IVehicle> _toInsert = [];
        private readonly List<IVehicle> _toUpdate = [];
        private readonly List<IRental> _rentalsToInsert = [];
        private readonly List<IRental> _rentalsToUpdate = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleRepository"/> class.
        /// </summary>
        /// <param name="database">Mongo database.</param>
        public VehicleRepository(IMongoDatabase database)
        {
            ArgumentNullException.ThrowIfNull(database);

            _vehicles = database.GetCollection<VehicleDocument>(VehiclesCollectionName);
            _rentals = database.GetCollection<RentalDocument>(RentalsCollectionName);
        }

        /// <summary>
        /// Gets or sets the Mongo session used by the pending operations when saving inside a transaction.
        /// </summary>
        internal IClientSessionHandle Session { get; set; }

        /// <summary>
        /// Adds a new vehicle to the fleet.
        /// </summary>
        /// <param name="vehicle">Vehicle to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task Add(IVehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            _toInsert.Add(vehicle);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets a vehicle by its identifier.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <returns>The vehicle.</returns>
        public async Task<IVehicle> GetBy(VehicleId vehicleId)
        {
            var document = await _vehicles
                .Find(vehicle => vehicle.Id == vehicleId.ToGuid())
                .FirstOrDefaultAsync();

            if (document is null)
            {
                return null;
            }

            TrackLoadedVersion(document);

            return MapToEntity(document);
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
                true => await _vehicles
                    .Find(vehicle => !vehicle.Rentals.Any(rental => rental.ReturnDate == null))
                    .ToListAsync(),
                false => await _vehicles
                    .Find(vehicle => vehicle.Rentals.Any(rental => rental.ReturnDate == null))
                    .ToListAsync(),
                _ => await _vehicles
                    .Find(FilterDefinition<VehicleDocument>.Empty)
                    .ToListAsync(),
            };

            foreach (var document in documents)
            {
                TrackLoadedVersion(document);
            }

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

            _toUpdate.Add(vehicle);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Queues a new rental for persistence.
        /// </summary>
        /// <param name="rental">Rental to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task AddRental(IRental rental)
        {
            ArgumentNullException.ThrowIfNull(rental);

            _rentalsToInsert.Add(rental);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Queues an updated rental for persistence.
        /// </summary>
        /// <param name="rental">Rental to update.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task UpdateRental(IRental rental)
        {
            ArgumentNullException.ThrowIfNull(rental);

            _rentalsToUpdate.Add(rental);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets a value indicating whether a person has an active rental in the fleet.
        /// </summary>
        /// <param name="person">Person to check.</param>
        /// <returns>Whether the person has an active rental.</returns>
        public async Task<bool> PersonHasActiveRental(Person person)
        {
            var count = await _rentals.CountDocumentsAsync(
                rental => rental.Person == person.Value && rental.ReturnDate == null);

            return count > 0;
        }

        /// <summary>
        /// Gets a value indicating whether a vehicle with the plate already exists in the fleet.
        /// </summary>
        /// <param name="plate">Plate to check.</param>
        /// <returns>Whether the plate already exists in the fleet.</returns>
        public async Task<bool> ExistsByPlate(Plate plate)
        {
            var count = await _vehicles.CountDocumentsAsync(vehicle => vehicle.Plate == plate.Value);

            return count > 0;
        }

        /// <summary>
        /// Persists all the pending changes.
        /// </summary>
        /// <returns>The number of persisted changes.</returns>
        internal async Task<int> SaveChanges()
        {
            try
            {
                return await SaveChangesCoreAsync();
            }
            catch (MongoCommandException ex) when (TransientTransactionErrorCodes.Contains(ex.Code))
            {
                throw new VehicleConcurrencyException(
                    "The vehicle was modified by another operation. Please try again.");
            }
        }

        private static VehicleDocument MapToDocument(IVehicle vehicle, long version)
        {
            return new VehicleDocument
            {
                Id = vehicle.Id.ToGuid(),
                Plate = vehicle.Plate.Value,
                Make = vehicle.Make.Value,
                Model = vehicle.Model.Value,
                ManufactureDate = vehicle.ManufactureDate.Value,
                Rentals = [.. vehicle.Rentals.GetRentals().Select(MapToDocument)],
                Version = version,
            };
        }

        private static RentalDocument MapToDocument(IRental rental)
        {
            return new RentalDocument
            {
                Id = rental.Id.ToGuid(),
                VehicleId = rental.VehicleId.ToGuid(),
                Person = rental.Person.Value,
                RentDate = rental.RentDate,
                ReturnDate = rental.ReturnDate,
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

        private async Task<int> SaveChangesCoreAsync()
        {
            var saved = 0;

            foreach (var vehicle in _toInsert)
            {
                try
                {
                    await InsertVehicleAsync(MapToDocument(vehicle, 1));
                }
                catch (MongoWriteException ex) when (ex.WriteError.Code == DuplicateKeyErrorCode)
                {
                    throw new VehicleAlreadyExistsException(
                        $"The vehicle with plate {vehicle.Plate} already exists in the fleet.");
                }

                saved++;
            }

            foreach (var vehicle in _toUpdate)
            {
                var expectedVersion = _loadedVersions.TryGetValue(vehicle.Id.ToGuid(), out var version)
                    ? version
                    : 1;

                var filter = Builders<VehicleDocument>.Filter.Eq("Id", vehicle.Id.ToGuid())
                    & Builders<VehicleDocument>.Filter.Eq("Version", expectedVersion);

                var result = await ReplaceVehicleAsync(filter, MapToDocument(vehicle, expectedVersion + 1));

                if (result.ModifiedCount == 0)
                {
                    throw new VehicleConcurrencyException(
                        "The vehicle was modified by another operation. Please try again.");
                }

                _loadedVersions[vehicle.Id.ToGuid()] = expectedVersion + 1;
                saved++;
            }

            foreach (var rental in _rentalsToInsert)
            {
                try
                {
                    await InsertRentalAsync(MapToDocument(rental));
                }
                catch (MongoWriteException ex) when (ex.WriteError.Code == DuplicateKeyErrorCode)
                {
                    throw new PersonAlreadyHasActiveRentalException("The person already has an active rental.");
                }

                saved++;
            }

            foreach (var rental in _rentalsToUpdate)
            {
                var filter = Builders<RentalDocument>.Filter.Eq("Id", rental.Id.ToGuid());
                var update = Builders<RentalDocument>.Update.Set(document => document.ReturnDate, rental.ReturnDate);
                await UpdateRentalAsync(filter, update);

                saved++;
            }

            _toInsert.Clear();
            _toUpdate.Clear();
            _rentalsToInsert.Clear();
            _rentalsToUpdate.Clear();

            return saved;
        }

        private void TrackLoadedVersion(VehicleDocument document)
        {
            _loadedVersions[document.Id] = document.Version > 0 ? document.Version : 1;
        }

        private async Task InsertVehicleAsync(VehicleDocument document)
        {
            if (Session is null)
            {
                await _vehicles.InsertOneAsync(document);
            }
            else
            {
                await _vehicles.InsertOneAsync(Session, document);
            }
        }

        private async Task<ReplaceOneResult> ReplaceVehicleAsync(
            FilterDefinition<VehicleDocument> filter,
            VehicleDocument document)
        {
            if (Session is null)
            {
                return await _vehicles.ReplaceOneAsync(filter, document, NoUpsertReplaceOptions);
            }

            return await _vehicles.ReplaceOneAsync(Session, filter, document, NoUpsertReplaceOptions);
        }

        private async Task InsertRentalAsync(RentalDocument document)
        {
            if (Session is null)
            {
                await _rentals.InsertOneAsync(document);
            }
            else
            {
                await _rentals.InsertOneAsync(Session, document);
            }
        }

        private async Task UpdateRentalAsync(
            FilterDefinition<RentalDocument> filter,
            UpdateDefinition<RentalDocument> update)
        {
            if (Session is null)
            {
                await _rentals.UpdateOneAsync(filter, update);
            }
            else
            {
                await _rentals.UpdateOneAsync(Session, filter, update);
            }
        }
    }
}
