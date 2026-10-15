using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Api.UseCases;
using GtMotive.Estimate.Microservice.Api.UseCases.CreateVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.GetVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.ListVehicles;
using GtMotive.Estimate.Microservice.Api.UseCases.RentVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle;
using GtMotive.Estimate.Microservice.Api.ViewModels;
using GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GtMotive.Estimate.Microservice.FunctionalTests.Specs
{
    public sealed class VehiclesSpec : FunctionalTestBase
    {
        private static readonly Guid UnknownVehicleId = Guid.NewGuid();

        public VehiclesSpec(CompositionRootTestFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public async Task CreateVehicle_WhenPlateIsNew_CreatesTheVehicleAndReturnsIt()
        {
            // Given
            var plate = UniquePlate();

            // When
            var actionResult = await SendAsync(new CreateVehicleRequest(CreateViewModel(plate)));

            // Then
            var result = actionResult.Should().BeAssignableTo<ObjectResult>().Subject;
            result.StatusCode.Should().Be(201);

            var vehicle = result.Value.Should().BeAssignableTo<VehicleViewModel>().Subject;
            vehicle.Plate.Should().Be(plate);
            vehicle.Make.Should().Be("Toyota");
            vehicle.Model.Should().Be("Corolla");
            vehicle.IsAvailable.Should().BeTrue();
        }

        [Fact]
        public async Task CreateVehicle_WhenPlateAlreadyExists_ReturnsConflict()
        {
            // Given
            var plate = UniquePlate();

            await SendAsync(new CreateVehicleRequest(CreateViewModel(plate)));

            // When
            var actionResult = await SendAsync(new CreateVehicleRequest(CreateViewModel(plate)));

            // Then
            actionResult.Should().BeAssignableTo<ConflictObjectResult>();
        }

        [Fact]
        public async Task GetVehicle_WhenVehicleDoesNotExist_ReturnsNotFound()
        {
            // When
            var actionResult = await GetAsync(UnknownVehicleId);

            // Then
            actionResult.Should().BeAssignableTo<NotFoundObjectResult>();
        }

        [Fact]
        public async Task RentVehicle_WhenVehicleIsAvailable_RentsTheVehicle()
        {
            // Given
            var vehicle = await CreateVehicleAsync(UniquePlate());

            // When
            var actionResult = await RentAsync(vehicle, UniquePerson());

            // Then
            var result = actionResult.Should().BeAssignableTo<OkObjectResult>().Subject;
            result.Value.Should().BeAssignableTo<VehicleViewModel>().Which.IsAvailable.Should().BeFalse();
        }

        [Fact]
        public async Task RentVehicle_WhenPersonAlreadyHasActiveRental_ReturnsConflict()
        {
            // Given
            var firstVehicle = await CreateVehicleAsync(UniquePlate());
            var secondVehicle = await CreateVehicleAsync(UniquePlate());
            var person = UniquePerson();

            (await RentAsync(firstVehicle, person)).Should().BeAssignableTo<OkObjectResult>();

            // When
            var actionResult = await RentAsync(secondVehicle, person);

            // Then
            actionResult.Should().BeAssignableTo<ConflictObjectResult>();
        }

        [Fact]
        public async Task RentVehicle_WhenVehicleDoesNotExist_ReturnsNotFound()
        {
            // When
            var actionResult = await SendAsync(
                new RentVehicleRequest(UnknownVehicleId, new RentVehicleViewModel { Person = UniquePerson() }));

            // Then
            actionResult.Should().BeAssignableTo<NotFoundObjectResult>();
        }

        [Fact]
        public async Task ReturnVehicle_WhenVehicleIsRented_ReturnsTheVehicle()
        {
            // Given
            var vehicle = await CreateVehicleAsync(UniquePlate());

            (await RentAsync(vehicle, UniquePerson())).Should().BeAssignableTo<OkObjectResult>();

            // When
            var actionResult = await ReturnAsync(vehicle);

            // Then
            var result = actionResult.Should().BeAssignableTo<OkObjectResult>().Subject;
            result.Value.Should().BeAssignableTo<VehicleViewModel>().Which.IsAvailable.Should().BeTrue();
        }

        [Fact]
        public async Task ReturnVehicle_WhenVehicleIsNotRented_ReturnsConflict()
        {
            // Given
            var vehicle = await CreateVehicleAsync(UniquePlate());

            // When
            var actionResult = await ReturnAsync(vehicle);

            // Then
            actionResult.Should().BeAssignableTo<ConflictObjectResult>();
        }

        [Fact]
        public async Task ListVehicles_WhenAvailableIsNotProvided_ReturnsAllVehicles()
        {
            // Given
            var availableVehicle = await CreateVehicleAsync(UniquePlate());
            var rentedVehicle = await CreateVehicleAsync(UniquePlate());

            (await RentAsync(rentedVehicle, UniquePerson())).Should().BeAssignableTo<OkObjectResult>();

            // When
            var actionResult = await SendAsync(new ListVehiclesRequest(default));

            // Then
            var vehicles = GetVehicles(actionResult);

            vehicles.Should().Contain(vehicle => vehicle.Plate == availableVehicle.Plate);
            vehicles.Should().Contain(vehicle => vehicle.Plate == rentedVehicle.Plate);
        }

        [Fact]
        public async Task ListVehicles_WhenAvailableIsFalse_ReturnsOnlyRentedVehicles()
        {
            // Given
            var availableVehicle = await CreateVehicleAsync(UniquePlate());
            var rentedVehicle = await CreateVehicleAsync(UniquePlate());

            (await RentAsync(rentedVehicle, UniquePerson())).Should().BeAssignableTo<OkObjectResult>();

            // When
            var actionResult = await SendAsync(new ListVehiclesRequest(false));

            // Then
            var vehicles = GetVehicles(actionResult);

            vehicles.Should().Contain(vehicle => vehicle.Plate == rentedVehicle.Plate);
            vehicles.Should().NotContain(vehicle => vehicle.Plate == availableVehicle.Plate);
        }

        private static List<VehicleViewModel> GetVehicles(IActionResult actionResult)
        {
            var result = actionResult.Should().BeAssignableTo<OkObjectResult>().Subject;

            return result.Value.Should().BeOfType<List<VehicleViewModel>>().Subject;
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

        private async Task<IActionResult> GetAsync(Guid vehicleId)
        {
            return await SendAsync(new GetVehicleRequest(vehicleId));
        }
    }
}
