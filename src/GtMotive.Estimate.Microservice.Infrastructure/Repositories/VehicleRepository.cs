using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles.ValueObjects;
using GtMotive.Estimate.Microservice.Infrastructure.Entities;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Entities;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure.Repositories
{
    /// <summary>
    /// Mongo implementation of the vehicle repository.
    /// </summary>
    /// <remarks>
    /// The repository is registered as scoped, so each unit of work gets its own instance: the
    /// pending changes and the loaded versions are never shared between requests. A vehicle must
    /// be loaded through this instance before one of its updates is queued, so that the update
    /// can be filtered by the version that was read.
    /// </remarks>
    public sealed class VehicleRepository : IVehicleRepository
    {
        private const int DuplicateKeyErrorCode = 11000;
        private const string VehiclesCollectionName = "vehicles";
        private const string RentalsCollectionName = "rentals";

        private static readonly ReplaceOptions NoUpsertReplaceOptions = new() { IsUpsert = false };

        private readonly IMongoCollection<VehicleDocument> _vehicles;
        private readonly IMongoCollection<RentalDocument> _rentals;
        private readonly ConcurrentDictionary<Guid, long> _loadedVersions = new();
        private readonly List<IVehicle> _toInsert = [];
        private readonly List<IVehicle> _toUpdate = [];
        private readonly List<IRental> _rentalsToInsert = [];
        private readonly List<IRental> _rentalsToUpdate = [];
        private SaveOutcome? _lastSaveOutcome;

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
                    .Find(vehicle => vehicle.ActiveRentalId == null)
                    .ToListAsync(),
                false => await _vehicles
                    .Find(vehicle => vehicle.ActiveRentalId != null)
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
        /// Gets a rental by its identifier.
        /// </summary>
        /// <param name="rentalId">Rental identifier.</param>
        /// <returns>The rental, or null if it does not exist.</returns>
        public async Task<IRental> GetRentalById(RentalId rentalId)
        {
            var document = await _rentals
                .Find(rental => rental.Id == rentalId.ToGuid())
                .FirstOrDefaultAsync();

            if (document is null)
            {
                return null;
            }

            return new RentalEntity(
                new RentalId(document.Id),
                new VehicleId(document.VehicleId),
                new Client(document.Client),
                document.RentDate,
                document.ReturnDate);
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
        /// Gets a value indicating whether a client has an active rental in the fleet.
        /// </summary>
        /// <param name="client">Client to check.</param>
        /// <returns>Whether the client has an active rental.</returns>
        public async Task<bool> ClientHasActiveRental(Client client)
        {
            var count = await _rentals.CountDocumentsAsync(
                rental => rental.Client == client.Value && rental.ReturnDate == null);

            return count > 0;
        }

        /// <summary>
        /// Gets a value indicating whether a vehicle with the plate already exists in the fleet.
        /// </summary>
        /// <param name="plate">Plate to check.</param>
        /// <returns>Whether the plate already exists.</returns>
        public async Task<bool> ExistsByPlate(Plate plate)
        {
            var count = await _vehicles.CountDocumentsAsync(vehicle => vehicle.Plate == plate.Value);

            return count > 0;
        }

        /// <summary>
        /// Persists all the pending changes.
        /// </summary>
        /// <remarks>
        /// The pending changes are kept after a failed save so the caller can replay them in a
        /// new transaction; they are only cleared once every write has been accepted.
        /// </remarks>
        /// <returns>The number of persisted changes.</returns>
        internal Task<int> SaveChanges()
        {
            return SaveChangesCoreAsync();
        }

        /// <summary>
        /// Verifies against the database whether every change of the last save was applied.
        /// </summary>
        /// <remarks>
        /// Used to resolve ambiguous transaction commit results, where the commit may or may not
        /// have reached the server. Because the save is atomic, either every change is applied
        /// or none is.
        /// </remarks>
        /// <returns>Whether every change of the last save is present in the database.</returns>
        internal async Task<bool> VerifyLastSaveOutcomeAsync()
        {
            var outcome = _lastSaveOutcome;

            if (outcome is null)
            {
                return false;
            }

            foreach (var vehicleId in outcome.VehicleInserts)
            {
                if (await _vehicles.CountDocumentsAsync(vehicle => vehicle.Id == vehicleId) == 0)
                {
                    return false;
                }
            }

            foreach (var update in outcome.VehicleUpdates)
            {
                var count = await _vehicles.CountDocumentsAsync(
                    vehicle => vehicle.Id == update.Id && vehicle.Version == update.Version);

                if (count == 0)
                {
                    return false;
                }
            }

            foreach (var rentalId in outcome.RentalInserts)
            {
                if (await _rentals.CountDocumentsAsync(rental => rental.Id == rentalId) == 0)
                {
                    return false;
                }
            }

            foreach (var update in outcome.RentalUpdates)
            {
                var count = await _rentals.CountDocumentsAsync(
                    rental => rental.Id == update.Id && rental.ReturnDate == update.ReturnDate);

                if (count == 0)
                {
                    return false;
                }
            }

            return true;
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
                ActiveRentalId = vehicle.ActiveRentalId?.ToGuid(),
                Version = version,
            };
        }

        private static RentalDocument MapToDocument(IRental rental)
        {
            return new RentalDocument
            {
                Id = rental.Id.ToGuid(),
                VehicleId = rental.VehicleId.ToGuid(),
                Client = rental.Client.Value,
                RentDate = rental.RentDate,
                ReturnDate = rental.ReturnDate,
            };
        }

        private static VehicleEntity MapToEntity(VehicleDocument document)
        {
            return new VehicleEntity(
                new VehicleId(document.Id),
                new Plate(document.Plate),
                new Make(document.Make),
                new Model(document.Model),
                new ManufactureDate(document.ManufactureDate),
                document.ActiveRentalId is { } activeRentalId ? new RentalId(activeRentalId) : null);
        }

        private async Task<int> SaveChangesCoreAsync()
        {
            var outcome = new SaveOutcome();
            var saved = 0;

            foreach (var vehicle in _toInsert)
            {
                try
                {
                    await InsertVehicleAsync(MapToDocument(vehicle, 1));
                }
                catch (MongoWriteException ex) when (ex.WriteError.Code == DuplicateKeyErrorCode)
                {
                    // The unique vehicles.Plate index is the authoritative backstop for duplicate plates.
                    throw new VehicleAlreadyExistsException(
                        $"The vehicle with plate {vehicle.Plate} already exists in the fleet.");
                }

                outcome.VehicleInserts.Add(vehicle.Id.ToGuid());
                saved++;
            }

            foreach (var vehicle in _toUpdate)
            {
                var expectedVersion = _loadedVersions.TryGetValue(vehicle.Id.ToGuid(), out var version)
                    ? version
                    : 1;

                // A missing vehicle and a stale version both surface as a zero match count. The service
                // exposes no delete operation, so both mean a concurrent operation won the race and the
                // caller is asked to refresh its state and try again.
                var filter = Builders<VehicleDocument>.Filter.Eq("Id", vehicle.Id.ToGuid())
                    & Builders<VehicleDocument>.Filter.Eq("Version", expectedVersion);

                var result = await ReplaceVehicleAsync(filter, MapToDocument(vehicle, expectedVersion + 1));

                if (result.ModifiedCount == 0)
                {
                    throw new VehicleConcurrencyException(
                        "The vehicle was modified by another operation. Please try again.");
                }

                _loadedVersions[vehicle.Id.ToGuid()] = expectedVersion + 1;
                outcome.VehicleUpdates.Add(new PendingVehicleUpdate(vehicle.Id.ToGuid(), expectedVersion + 1));
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
                    // The unique rentals.Client partial index (active rentals only) is the authoritative
                    // backstop for one active rental per client.
                    throw new ClientAlreadyHasActiveRentalException("The client already has an active rental.");
                }

                outcome.RentalInserts.Add(rental.Id.ToGuid());
                saved++;
            }

            foreach (var rental in _rentalsToUpdate)
            {
                var filter = Builders<RentalDocument>.Filter.Eq("Id", rental.Id.ToGuid());
                var update = Builders<RentalDocument>.Update.Set(document => document.ReturnDate, rental.ReturnDate);
                await UpdateRentalAsync(filter, update);

                outcome.RentalUpdates.Add(new PendingRentalUpdate(rental.Id.ToGuid(), rental.ReturnDate));
                saved++;
            }

            _lastSaveOutcome = outcome;

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

        /// <summary>
        /// Snapshot of the changes persisted by the last save, used to verify ambiguous transaction commits.
        /// </summary>
        private sealed class SaveOutcome
        {
            /// <summary>
            /// Gets the identifiers of the inserted vehicles.
            /// </summary>
            public List<Guid> VehicleInserts { get; } = [];

            /// <summary>
            /// Gets the vehicles replaced with a new version.
            /// </summary>
            public List<PendingVehicleUpdate> VehicleUpdates { get; } = [];

            /// <summary>
            /// Gets the identifiers of the inserted rentals.
            /// </summary>
            public List<Guid> RentalInserts { get; } = [];

            /// <summary>
            /// Gets the rentals updated with a new return date.
            /// </summary>
            public List<PendingRentalUpdate> RentalUpdates { get; } = [];
        }

        /// <summary>
        /// Vehicle update persisted with the given version.
        /// </summary>
        /// <param name="Id">Vehicle identifier.</param>
        /// <param name="Version">Version written by the save.</param>
        private sealed record PendingVehicleUpdate(Guid Id, long Version);

        /// <summary>
        /// Rental update persisted with the given return date.
        /// </summary>
        /// <param name="Id">Rental identifier.</param>
        /// <param name="ReturnDate">Return date written by the save.</param>
        private sealed record PendingRentalUpdate(Guid Id, DateTime? ReturnDate);
    }
}
