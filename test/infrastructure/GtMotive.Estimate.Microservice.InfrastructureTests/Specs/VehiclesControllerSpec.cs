using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure;
using Xunit;

namespace GtMotive.Estimate.Microservice.InfrastructureTests.Specs
{
    public sealed class VehiclesControllerSpec : InfrastructureTestBase
    {
        public VehiclesControllerSpec(GenericInfrastructureTestServerFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public async Task CreateVehicle_WhenBodyIsEmpty_ReturnsBadRequest()
        {
            using var client = Fixture.Server.CreateClient();
            using var content = new StringContent("{}", Encoding.UTF8, "application/json");

            using var response = await client.PostAsync(new Uri("/api/vehicles", UriKind.Relative), content);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateVehicle_WhenManufactureDateIsOlderThanFiveYears_ReturnsBadRequest()
        {
            using var client = Fixture.Server.CreateClient();

            var vehicle = new
            {
                plate = "ABC-123",
                make = "Toyota",
                model = "Corolla",
                manufactureDate = DateTime.UtcNow.AddYears(-6).ToString("o"),
            };

            using var response = await client.PostAsJsonAsync("/api/vehicles", vehicle);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateVehicle_WhenManufactureDateIsInTheFuture_ReturnsBadRequest()
        {
            using var client = Fixture.Server.CreateClient();

            var vehicle = new
            {
                plate = "ABC-123",
                make = "Toyota",
                model = "Corolla",
                manufactureDate = DateTime.UtcNow.AddDays(1).ToString("o"),
            };

            using var response = await client.PostAsJsonAsync("/api/vehicles", vehicle);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
