using System;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles.ValueObjects;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.Domain
{
    public sealed class VehicleTests
    {
        private static readonly DateTime RentDate = DateTime.UtcNow;
        private static readonly DateTime ReturnDate = DateTime.UtcNow;

        [Fact]
        public void Rent_WhenTheVehicleIsAvailable_RentsTheVehicle()
        {
            // Given
            var vehicle = new TestVehicle();

            // When
            var rental = vehicle.Rent(new StubRentalFactory(), NewClient(), RentDate);

            // Then
            rental.Should().NotBeNull();
            vehicle.IsAvailable.Should().BeFalse();
            vehicle.ActiveRentalId.Should().Be(rental.Id);
        }

        [Fact]
        public void Rent_WhenTheVehicleIsAlreadyRented_ThrowsVehicleAlreadyRentedException()
        {
            // Given
            var (vehicle, _) = RentAVehicle();

            // When
            var act = () => vehicle.Rent(new StubRentalFactory(), NewClient(), RentDate);

            // Then
            act.Should().Throw<VehicleAlreadyRentedException>();
        }

        [Fact]
        public void Return_WhenThereIsAnActiveRental_ReturnsTheActiveRental()
        {
            // Given
            var (vehicle, rental) = RentAVehicle();

            // When
            vehicle.Return(rental.Object, ReturnDate);

            // Then
            rental.Verify(mock => mock.Return(ReturnDate), Times.Once);
            vehicle.IsAvailable.Should().BeTrue();
            vehicle.ActiveRentalId.Should().BeNull();
        }

        [Fact]
        public void Return_WhenTheVehicleIsNotRented_ThrowsVehicleNotRentedException()
        {
            // Given
            var vehicle = new TestVehicle();
            var rental = new Mock<IRental>();

            // When
            var act = () => vehicle.Return(rental.Object, ReturnDate);

            // Then
            act.Should().Throw<VehicleNotRentedException>();
        }

        [Fact]
        public void Return_WhenTheRentalIsNotTheActiveOne_ThrowsVehicleNotRentedException()
        {
            // Given
            var (vehicle, activeRental) = RentAVehicle();
            var otherRental = new Mock<IRental>();

            // When
            var act = () => vehicle.Return(otherRental.Object, ReturnDate);

            // Then
            act.Should().Throw<VehicleNotRentedException>();
            activeRental.Verify(mock => mock.Return(It.IsAny<DateTime>()), Times.Never);
        }

        private static (TestVehicle Vehicle, Mock<IRental> Rental) RentAVehicle()
        {
            var vehicle = new TestVehicle();
            var factory = new StubRentalFactory();

            vehicle.Rent(factory, NewClient(), RentDate);

            return (vehicle, factory.Created);
        }

        private static Client NewClient()
        {
            return new Client("client@example.com");
        }

        private sealed class TestVehicle : Vehicle
        {
        }

        private sealed class StubRentalFactory : IRentalFactory
        {
            public Mock<IRental> Created { get; private set; }

            public IRental NewRental(VehicleId vehicleId, Client client)
            {
                Created = new Mock<IRental>();
                Created
                    .Setup(mock => mock.Id)
                    .Returns(new RentalId(Guid.NewGuid()));

                return Created.Object;
            }
        }
    }
}
