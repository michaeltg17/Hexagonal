using System;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace GtMotive.Estimate.Microservice.FunctionalTests.Specs
{
    /// <summary>
    /// Tests of the Mongo database initializer: idempotency and invariant enforcement.
    /// </summary>
    public sealed class MongoDbInitializerSpec : FunctionalTestBase
    {
        private const int DuplicateKeyErrorCode = 11000;
        private const string ClientIndexName = "Client_1";

        public MongoDbInitializerSpec(CompositionRootTestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Running the initializer a second time on an already initialized database must succeed.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [Fact]
        public async Task InitializeAsync_WhenRunAgainOnInitializedDatabase_CompletesSuccessfully()
        {
            // Given: the test base has already initialized the database.

            // When
            var act = () => Fixture.InitializeDatabaseAsync();

            // Then
            await act.Should().NotThrowAsync();
        }

        /// <summary>
        /// If the stored data already violates the one-active-rental-per-client invariant,
        /// the initializer fails loudly instead of silently picking a winner.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [Fact]
        public async Task InitializeAsync_WhenDataViolatesClientUniqueness_FailsLoudly()
        {
            // Given: the index is removed and two active rentals of the same client exist.
            await Fixture.DropRentalIndexAsync(ClientIndexName);

            var clientEmail = $"client-{Guid.NewGuid():N}@example.com";

            for (var attempt = 0; attempt < 2; attempt++)
            {
                await Fixture.InsertRentalDocumentAsync(
                    new BsonDocument
                    {
                        { "Id", CompositionRootTestFixture.SerializeGuid(Guid.NewGuid()) },
                        { "VehicleId", CompositionRootTestFixture.SerializeGuid(Guid.NewGuid()) },
                        { "Client", clientEmail },
                        { "RentDate", DateTime.UtcNow },
                        { "ReturnDate", BsonNull.Value },
                    });
            }

            // When
            var act = () => Fixture.InitializeDatabaseAsync();

            // Then
            var exception = (await act.Should().ThrowAsync<MongoCommandException>()).Which;

            exception.Code.Should().Be(DuplicateKeyErrorCode);
        }
    }
}
