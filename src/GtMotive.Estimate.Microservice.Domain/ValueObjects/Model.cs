using System;
using GtMotive.Estimate.Microservice.Domain.Exceptions;

namespace GtMotive.Estimate.Microservice.Domain.ValueObjects
{
    /// <summary>
    /// Value object that represents the model of a vehicle.
    /// </summary>
    public readonly record struct Model
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> class.
        /// </summary>
        /// <param name="value">Model value.</param>
        public Model(string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValueShouldNotBeEmptyException("The model can not be empty.");
            }

            Value = value.Trim();
        }

        /// <summary>
        /// Gets the model value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Returns the model value as a string.
        /// </summary>
        /// <returns>The model value.</returns>
        public override string ToString()
        {
            return Value;
        }
    }
}
