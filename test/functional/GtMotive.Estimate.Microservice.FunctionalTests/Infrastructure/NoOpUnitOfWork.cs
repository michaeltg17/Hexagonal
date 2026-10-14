using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure
{
    internal sealed class NoOpUnitOfWork : IUnitOfWork
    {
        public Task<int> Save()
        {
            return Task.FromResult(0);
        }
    }
}
