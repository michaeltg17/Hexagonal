using System;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Domain.Exceptions;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.Domain
{
    public sealed class VehicleTests
    {
        private static readonly DateTime ReturnDate = DateTime.UtcNow;

        [Fact]
        public void Return_WhenThereIsAnActiveRental_ReturnsTheActiveRental()
        {
            // Given
            var vehicle = new TestVehicle();
            var rental = new Mock<IRental>();

            rental
                .Setup(mock => mock.IsActive)
                .Returns(true);

            vehicle.Rentals.Add(rental.Object);

            // When
            vehicle.Return(ReturnDate);

            // Then
            rental.Verify(mock => mock.Return(ReturnDate), Times.Once);
        }

        [Fact]
        public void Return_WhenTheVehicleIsNotRented_ThrowsVehicleNotRentedException()
        {
            // Given
            var vehicle = new TestVehicle();

            // When
            var act = () => vehicle.Return(ReturnDate);

            // Then
            act.Should().Throw<VehicleNotRentedException>();
        }

        [Fact]
        public void Return_WhenAllRentalsAreReturned_ThrowsVehicleNotRentedException()
        {
            // Given
            var vehicle = new TestVehicle();
            var returnedRental = new Mock<IRental>();

            returnedRental
                .Setup(mock => mock.IsActive)
                .Returns(false);

            vehicle.Rentals.Add(returnedRental.Object);

            // When
            var act = () => vehicle.Return(ReturnDate);

            // Then
            act.Should().Throw<VehicleNotRentedException>();
        }

        [Fact]
        public void Return_WhenThereIsAHistoryAndAnActiveRental_ReturnsOnlyTheActiveRental()
        {
            // Given
            var vehicle = new TestVehicle();
            var returnedRental = new Mock<IRental>();
            var activeRental = new Mock<IRental>();

            returnedRental
                .Setup(mock => mock.IsActive)
                .Returns(false);

            activeRental
                .Setup(mock => mock.IsActive)
                .Returns(true);

            vehicle.Rentals.Add(returnedRental.Object);
            vehicle.Rentals.Add(activeRental.Object);

            // When
            vehicle.Return(ReturnDate);

            // Then
            activeRental.Verify(mock => mock.Return(ReturnDate), Times.Once);
            returnedRental.Verify(mock => mock.Return(It.IsAny<DateTime>()), Times.Never);
        }

        private sealed class TestVehicle : Vehicle
        {
        }
    }
}
