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
            var clientEmails = Enumerable.Range(0, ConcurrentRenters).Select(_ => UniqueClientEmail()).ToList();

            // When
            var results = await Task.WhenAll(
                clientEmails.Select(clientEmail => RentAsync(vehicle, clientEmail)));

            // Then
            results.Count(result => result is OkObjectResult).Should().Be(1);
            results.Count(result => result is ConflictObjectResult).Should().Be(ConcurrentRenters - 1);

            // Only the winning transaction committed: one active rental, no orphan
            // rental documents and the vehicle advanced exactly one version.
            (await Fixture.CountActiveRentalsAsync()).Should().Be(1);
            (await Fixture.CountRentalsAsync()).Should().Be(1);
            (await Fixture.GetVehicleVersionAsync(vehicle.Id)).Should().Be(2);
        }

        [Fact]
        public async Task RentVehicle_WhenSameClientRentsConcurrently_OnlyOneRentSucceeds()
        {
            // Given
            var vehicles = await Task.WhenAll(
                Enumerable.Range(0, 2).Select(_ => CreateVehicleAsync(UniquePlate())));
            var clientEmail = UniqueClientEmail();

            // When
            var results = await Task.WhenAll(
                vehicles.Select(vehicle => RentAsync(vehicle, clientEmail)));

            // Then
            results.Count(result => result is OkObjectResult).Should().Be(1);
            results.Count(result => result is ConflictObjectResult).Should().Be(1);

            // The client holds exactly one active rental and the losing transaction
            // left no state behind.
            (await Fixture.CountActiveRentalsAsync()).Should().Be(1);
            (await Fixture.CountRentalsAsync()).Should().Be(1);
        }

        [Fact]
        public async Task ReturnVehicle_WhenReturnedConcurrently_OnlyOneReturnSucceeds()
        {
            // Given
            var vehicle = await CreateVehicleAsync(UniquePlate());

            (await RentAsync(vehicle, UniqueClientEmail())).Should().BeAssignableTo<OkObjectResult>();

            // When
            var results = await Task.WhenAll(
                Enumerable.Range(0, 2).Select(_ => ReturnAsync(vehicle)));

            // Then
            results.Count(result => result is OkObjectResult).Should().Be(1);
            results.Count(result => result is ConflictObjectResult).Should().Be(1);

            // Exactly one return committed: no active rental and the vehicle
            // advanced from version 2 (rent) to version 3 (return) only.
            (await Fixture.CountActiveRentalsAsync()).Should().Be(0);
            (await Fixture.CountRentalsAsync()).Should().Be(1);
            (await Fixture.GetVehicleVersionAsync(vehicle.Id)).Should().Be(3);
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

        private static string UniqueClientEmail()
        {
            return $"client-{Guid.NewGuid():N}@example.com";
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

        private async Task<IActionResult> RentAsync(VehicleViewModel vehicle, string clientEmail)
        {
            return await SendAsync(new RentVehicleRequest(vehicle.Id, new RentVehicleViewModel { ClientEmail = clientEmail }));
        }

        private async Task<IActionResult> ReturnAsync(VehicleViewModel vehicle)
        {
            return await SendAsync(new ReturnVehicleRequest(vehicle.Id));
        }
    }
}
