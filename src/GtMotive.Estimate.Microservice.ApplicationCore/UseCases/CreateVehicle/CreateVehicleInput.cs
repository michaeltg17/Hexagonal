using System;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle
{
    /// <summary>
    /// Input message of the create vehicle use case.
    /// </summary>
    public sealed record CreateVehicleInput : IUseCaseInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateVehicleInput"/> class.
        /// </summary>
        /// <param name="plate">Plate.</param>
        /// <param name="make">Make.</param>
        /// <param name="model">Model.</param>
        /// <param name="manufactureDate">Manufacture date.</param>
        public CreateVehicleInput(string plate, string make, string model, DateTime manufactureDate)
        {
            Plate = plate;
            Make = make;
            Model = model;
            ManufactureDate = manufactureDate;
        }

        /// <summary>
        /// Gets the plate.
        /// </summary>
        public string Plate { get; init; }

        /// <summary>
        /// Gets the make.
        /// </summary>
        public string Make { get; init; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public string Model { get; init; }

        /// <summary>
        /// Gets the manufacture date.
        /// </summary>
        public DateTime ManufactureDate { get; init; }
    }
}
