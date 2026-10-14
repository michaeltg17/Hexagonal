using System;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Services;
using GtMotive.Estimate.Microservice.Domain.ValueObjects;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore
{
    public sealed class CreateVehicleUseCaseTests
    {
        private readonly Mock<IVehicleFactory> _vehicleFactory = new();
        private readonly Mock<IRentalFactory> _rentalFactory = new();
        private readonly Mock<IVehicleRepository> _vehicleRepository = new();
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly Mock<ICreateVehicleOutputPort> _outputPort = new();
        private readonly CreateVehicleUseCase _createVehicleUseCase;

        public CreateVehicleUseCaseTests()
        {
            var vehicleService = new VehicleService(
                _vehicleFactory.Object,
                _rentalFactory.Object,
                _vehicleRepository.Object);

            _createVehicleUseCase = new CreateVehicleUseCase(
                vehicleService,
                _unitOfWork.Object,
                _outputPort.Object);
        }

        [Fact]
        public async Task Execute_WhenInputIsValid_CreatesTheVehicleAndHandlesStandardOutput()
        {
            CreateVehicleOutput capturedOutput = default;

            var vehicle = CreateVehicleStub(DateTime.UtcNow.AddDays(-1));

            _vehicleFactory
                .Setup(factory => factory.NewVehicle(It.IsAny<Plate>(), It.IsAny<Make>(), It.IsAny<Model>(), It.IsAny<ManufactureDate>()))
                .Returns(vehicle);
            _vehicleRepository
                .Setup(repository => repository.ExistsByPlate(It.IsAny<Plate>()))
                .ReturnsAsync(false);
            _unitOfWork
                .Setup(unitOfWork => unitOfWork.Save())
                .ReturnsAsync(1);
            _outputPort
                .Setup(port => port.StandardHandle(It.IsAny<CreateVehicleOutput>()))
                .Callback<CreateVehicleOutput>(output => capturedOutput = output);

            await _createVehicleUseCase.Execute(
                new CreateVehicleInput("ABC-123", "Toyota", "Corolla", vehicle.ManufactureDate.Value));

            _vehicleRepository.Verify(repository => repository.Add(It.IsAny<IVehicle>()), Times.Once);
            _unitOfWork.Verify(unitOfWork => unitOfWork.Save(), Times.Once);
            _outputPort.Verify(port => port.StandardHandle(It.IsAny<CreateVehicleOutput>()), Times.Once);
            _outputPort.Verify(port => port.ConflictHandle(It.IsAny<string>()), Times.Never);
            _outputPort.Verify(port => port.BadRequestHandle(It.IsAny<string>()), Times.Never);

            capturedOutput.Should().NotBeNull();
            capturedOutput.Vehicle.Plate.Should().Be("ABC-123");
            capturedOutput.Vehicle.Make.Should().Be("Toyota");
            capturedOutput.Vehicle.Model.Should().Be("Corolla");
            capturedOutput.Vehicle.IsAvailable.Should().BeTrue();
        }

        [Fact]
        public async Task Execute_WhenPlateAlreadyExists_HandlesConflict()
        {
            _vehicleRepository
                .Setup(repository => repository.ExistsByPlate(It.IsAny<Plate>()))
                .ReturnsAsync(true);

            await _createVehicleUseCase.Execute(
                new CreateVehicleInput("ABC-123", "Toyota", "Corolla", DateTime.UtcNow.AddDays(-1)));

            _outputPort.Verify(
                port => port.ConflictHandle(It.Is<string>(message => message.Contains("ABC-123"))),
                Times.Once);
            _outputPort.Verify(port => port.StandardHandle(It.IsAny<CreateVehicleOutput>()), Times.Never);
            _outputPort.Verify(port => port.BadRequestHandle(It.IsAny<string>()), Times.Never);
            _unitOfWork.Verify(unitOfWork => unitOfWork.Save(), Times.Never);
            _vehicleFactory.Verify(
                factory => factory.NewVehicle(It.IsAny<Plate>(), It.IsAny<Make>(), It.IsAny<Model>(), It.IsAny<ManufactureDate>()),
                Times.Never);
        }

        [Fact]
        public async Task Execute_WhenManufactureDateIsOlderThanFiveYears_HandlesBadRequest()
        {
            await _createVehicleUseCase.Execute(
                new CreateVehicleInput("ABC-123", "Toyota", "Corolla", DateTime.UtcNow.AddYears(-6)));

            _outputPort.Verify(port => port.BadRequestHandle(It.IsAny<string>()), Times.Once);
            _outputPort.Verify(port => port.StandardHandle(It.IsAny<CreateVehicleOutput>()), Times.Never);
            _outputPort.Verify(port => port.ConflictHandle(It.IsAny<string>()), Times.Never);
            _vehicleRepository.Verify(repository => repository.ExistsByPlate(It.IsAny<Plate>()), Times.Never);
            _unitOfWork.Verify(unitOfWork => unitOfWork.Save(), Times.Never);
        }

        private static IVehicle CreateVehicleStub(DateTime manufactureDate)
        {
            var vehicle = new Mock<IVehicle>();

            vehicle
                .Setup(v => v.Id)
                .Returns(new VehicleId(Guid.NewGuid()));
            vehicle
                .Setup(v => v.Plate)
                .Returns(new Plate("ABC-123"));
            vehicle
                .Setup(v => v.Make)
                .Returns(new Make("Toyota"));
            vehicle
                .Setup(v => v.Model)
                .Returns(new Model("Corolla"));
            vehicle
                .Setup(v => v.ManufactureDate)
                .Returns(new ManufactureDate(manufactureDate));
            vehicle
                .Setup(v => v.IsAvailable)
                .Returns(true);

            return vehicle.Object;
        }
    }
}
