using System;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases
{
    /// <summary>
    /// Read model of a vehicle.
    /// </summary>
    public sealed record VehicleDto
    {
        /// <summary>
        /// Gets the vehicle identifier.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Gets the plate.
        /// </summary>
        public string Plate { get; init; } = string.Empty;

        /// <summary>
        /// Gets the make.
        /// </summary>
        public string Make { get; init; } = string.Empty;

        /// <summary>
        /// Gets the model.
        /// </summary>
        public string Model { get; init; } = string.Empty;

        /// <summary>
        /// Gets the manufacture date.
        /// </summary>
        public DateTime ManufactureDate { get; init; }

        /// <summary>
        /// Gets a value indicating whether the vehicle is available.
        /// </summary>
        public bool IsAvailable { get; init; }
    }
}
