using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions;
using GtMotive.Estimate.Microservice.Infrastructure.Repositories;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure
{
    /// <summary>
    /// Unit of work that persists the changes buffered by the repositories.
    /// </summary>
    public sealed class UnitOfWork : IUnitOfWork
    {
        private const int MaxTransactionAttempts = 3;
        private const int UnknownTransactionCommitResultErrorCode = 69770;
        private const string TransientTransactionErrorLabel = "TransientTransactionError";
        private const string UnknownTransactionCommitResultLabel = "UnknownTransactionCommitResult";

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

        private readonly IMongoClient _client;
        private readonly VehicleRepository _vehicleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="client">Mongo client.</param>
        /// <param name="vehicleRepository">Vehicle repository.</param>
        public UnitOfWork(IMongoClient client, VehicleRepository vehicleRepository)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        }

        /// <summary>
        /// Applies all database changes atomically in a single transaction.
        /// </summary>
        /// <remarks>
        /// Transient transaction failures abort the attempt and replay the same buffered changes
        /// in a fresh transaction, up to <see cref="MaxTransactionAttempts"/> times: the repository
        /// keeps the pending changes after a failed save, so the replay is safe. If the commit
        /// result is unknown the changes are verified against the database before reporting
        /// success or a conflict.
        /// </remarks>
        /// <returns>Number of affected rows.</returns>
        public async Task<int> Save()
        {
            using var session = await _client.StartSessionAsync();

            try
            {
                var attempt = 0;

                while (true)
                {
                    attempt++;

                    session.StartTransaction();
                    _vehicleRepository.Session = session;

                    var saved = 0;

                    try
                    {
                        saved = await _vehicleRepository.SaveChanges();

                        await session.CommitTransactionAsync();

                        return saved;
                    }
                    catch (MongoCommandException ex) when (IsUnknownTransactionCommitResult(ex))
                    {
                        // The commit may or may not have been applied. Resolve the ambiguity by
                        // reading the persisted state back instead of assuming an outcome.
                        var applied = await _vehicleRepository.VerifyLastSaveOutcomeAsync();
                        await TryAbortTransactionAsync(session);

                        if (!applied)
                        {
                            throw new VehicleConcurrencyException(
                                "The vehicle was modified by another operation. Please try again.");
                        }

                        return saved;
                    }
                    catch (MongoCommandException ex) when (IsTransientTransactionError(ex))
                    {
                        await TryAbortTransactionAsync(session);

                        if (attempt >= MaxTransactionAttempts)
                        {
                            throw new VehicleConcurrencyException(
                                "The vehicle was modified by another operation. Please try again.");
                        }
                    }
                    catch
                    {
                        await TryAbortTransactionAsync(session);
                        throw;
                    }
                    finally
                    {
                        _vehicleRepository.Session = null;
                    }
                }
            }
            finally
            {
                _vehicleRepository.Session = null;
            }
        }

        private static bool IsUnknownTransactionCommitResult(MongoCommandException exception)
        {
            return exception.HasErrorLabel(UnknownTransactionCommitResultLabel)
                || exception.Code == UnknownTransactionCommitResultErrorCode;
        }

        private static bool IsTransientTransactionError(MongoCommandException exception)
        {
            return exception.HasErrorLabel(TransientTransactionErrorLabel)
                || TransientTransactionErrorCodes.Contains(exception.Code);
        }

        private static async Task TryAbortTransactionAsync(IClientSessionHandle session)
        {
            try
            {
                await session.AbortTransactionAsync();
            }
            catch (MongoException)
            {
                // The transaction may already have been aborted, committed or timed out.
            }
        }
    }
}
