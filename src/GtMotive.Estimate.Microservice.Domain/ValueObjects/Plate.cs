using System;
using GtMotive.Estimate.Microservice.Domain.Exceptions;

namespace GtMotive.Estimate.Microservice.Domain.ValueObjects
{
    /// <summary>
    /// Value object that represents the plate of a vehicle.
    /// </summary>
    public readonly record struct Plate
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
                throw new ValueShouldNotBeEmptyException("The plate can not be empty.");
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
