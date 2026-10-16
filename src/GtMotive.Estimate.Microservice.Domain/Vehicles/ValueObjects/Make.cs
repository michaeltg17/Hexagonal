using System;
using GtMotive.Estimate.Microservice.Domain.Exceptions;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles.ValueObjects
{
    /// <summary>
    /// Value object that represents the make of a vehicle.
    /// </summary>
    public sealed record Make
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Make"/> class.
        /// </summary>
        /// <param name="value">Make value.</param>
        public Make(string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValueShouldNotBeEmptyException("The vehicle make can not be empty.");
            }

            Value = value.Trim();
        }

        /// <summary>
        /// Gets the make value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Returns the make value as a string.
        /// </summary>
        /// <returns>The make value.</returns>
        public override string ToString()
        {
            return Value;
        }
    }
}
