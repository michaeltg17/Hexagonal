using System;
using System.Threading.Tasks;
using Xunit;

namespace GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure
{
    [Collection(TestCollections.Functional)]
    public abstract class FunctionalTestBase : IAsyncLifetime
    {
        protected FunctionalTestBase(CompositionRootTestFixture fixture)
        {
            Fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        protected CompositionRootTestFixture Fixture { get; }

        public async Task InitializeAsync()
        {
            await Fixture.ResetDatabaseAsync();
        }

        public async Task DisposeAsync()
        {
            await Task.CompletedTask;
        }
    }
}
