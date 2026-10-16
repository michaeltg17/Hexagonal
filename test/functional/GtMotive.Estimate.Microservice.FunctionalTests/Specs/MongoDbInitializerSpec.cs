using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure;
using Xunit;

namespace GtMotive.Estimate.Microservice.FunctionalTests.Specs
{
    /// <summary>
    /// Tests of the Mongo database initializer: idempotency and invariant enforcement.
    /// </summary>
    public sealed class MongoDbInitializerSpec(CompositionRootTestFixture fixture) : FunctionalTestBase(fixture)
    {
        /// <summary>
        /// Running the initializer a second time on an already initialized database must succeed.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [Fact]
        public async Task InitializeAsync_WhenRunAgainOnInitializedDatabase_CompletesSuccessfully()
        {
            // Given: the test base has already initialized the database.

            // When
#pragma warning disable IDE0200 // Remove unnecessary lambda expression
            var act = () => Fixture.InitializeDatabaseAsync();
#pragma warning restore IDE0200 // Remove unnecessary lambda expression

            // Then
            await act.Should().NotThrowAsync();
        }
    }
}
