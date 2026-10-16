using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Api.UseCases;
using GtMotive.Estimate.Microservice.Api.UseCases.CreateVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.RentVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle;
using GtMotive.Estimate.Microservice.Api.ViewModels;
using GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GtMotive.Estimate.Microservice.FunctionalTests.Specs
{
    /// <summary>
    /// Concurrency tests that exercise the rental invariants under concurrent requests against Mongo.
    /// </summary>
    public sealed class ConcurrencySpec : FunctionalTestBase
    {
        private const int ConcurrentRenters = 5;

        public ConcurrencySpec(CompositionRootTestFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public async Task RentVehicle_WhenSameVehicleIsRentedConcurrently_OnlyOneRentSucceeds()
        {
            // Given
            var vehicle = await CreateVehicleAsync(UniquePlate());
            var persons = Enumerable.Range(0, ConcurrentRenters).Select(_ => UniquePerson()).ToList();

            // When
            var results = await Task.WhenAll(
                persons.Select(person => RentAsync(vehicle, person)));

            // Then
            results.Count(result => result is OkObjectResult).Should().Be(1);
            results.Count(result => result is ConflictObjectResult).Should().Be(ConcurrentRenters - 1);
        }

        [Fact]
        public async Task RentVehicle_WhenSamePersonRentsConcurrently_OnlyOneRentSucceeds()
        {
            // Given
            var vehicles = await Task.WhenAll(
                Enumerable.Range(0, 2).Select(_ => CreateVehicleAsync(UniquePlate())));
            var person = UniquePerson();

            // When
            var results = await Task.WhenAll(
                vehicles.Select(vehicle => RentAsync(vehicle, person)));

            // Then
            results.Count(result => result is OkObjectResult).Should().Be(1);
            results.Count(result => result is ConflictObjectResult).Should().Be(1);
        }

        [Fact]
        public async Task ReturnVehicle_WhenReturnedConcurrently_OnlyOneReturnSucceeds()
        {
            // Given
            var vehicle = await CreateVehicleAsync(UniquePlate());

            (await RentAsync(vehicle, UniquePerson())).Should().BeAssignableTo<OkObjectResult>();

            // When
            var results = await Task.WhenAll(
                Enumerable.Range(0, 2).Select(_ => ReturnAsync(vehicle)));

            // Then
            results.Count(result => result is OkObjectResult).Should().Be(1);
            results.Count(result => result is ConflictObjectResult).Should().Be(1);
        }

        private static CreateVehicleViewModel CreateViewModel(string plate)
        {
            return new CreateVehicleViewModel
            {
                Plate = plate,
                Make = "Toyota",
                Model = "Corolla",
                ManufactureDate = DateTime.UtcNow.AddDays(-1),
            };
        }

        private static string UniquePlate()
        {
            return $"TEST-{Guid.NewGuid():N}";
        }

        private static string UniquePerson()
        {
            return $"Person {Guid.NewGuid():N}";
        }

        private async Task<IActionResult> SendAsync<TRequest>(TRequest request)
            where TRequest : IRequest<IWebApiPresenter>
        {
            IWebApiPresenter presenter = default;

            await Fixture.UsingHandlerForRequestResponse<TRequest, IWebApiPresenter>(
                async handler => presenter = await handler.Handle(request, CancellationToken.None));

            presenter.Should().NotBeNull();

            return presenter.ActionResult;
        }

        private async Task<VehicleViewModel> CreateVehicleAsync(string plate)
        {
            var actionResult = await SendAsync(new CreateVehicleRequest(CreateViewModel(plate)));
            var result = actionResult.Should().BeAssignableTo<ObjectResult>().Subject;

            result.StatusCode.Should().Be(201);

            return result.Value.Should().BeAssignableTo<VehicleViewModel>().Subject;
        }

        private async Task<IActionResult> RentAsync(VehicleViewModel vehicle, string person)
        {
            return await SendAsync(new RentVehicleRequest(vehicle.Id, new RentVehicleViewModel { Person = person }));
        }

        private async Task<IActionResult> ReturnAsync(VehicleViewModel vehicle)
        {
            return await SendAsync(new ReturnVehicleRequest(vehicle.Id));
        }
    }
}
