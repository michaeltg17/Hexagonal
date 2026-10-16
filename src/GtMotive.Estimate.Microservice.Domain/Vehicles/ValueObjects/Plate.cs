using System;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles.ValueObjects
{
    /// <summary>
    /// Value object that represents the plate of a vehicle.
    /// </summary>
    public sealed record Plate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Plate"/> class.
        /// </summary>
        /// <param name="value">Plate value.</param>
        public Plate(string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValueShouldNotBeEmptyException("The vehicle plate can not be empty.");
            }

            Value = value.Trim();
        }

        /// <summary>
        /// Gets the plate value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Returns the plate value as a string.
        /// </summary>
        /// <returns>The plate value.</returns>
        public override string ToString()
        {
            return Value;
        }
    }
}
