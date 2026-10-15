using System;
using System.Runtime.CompilerServices;
using GtMotive.Estimate.Microservice.Domain.Exceptions;

namespace GtMotive.Estimate.Microservice.Domain.ValueObjects
{
    /// <summary>
    /// Base value object that wraps a validated non-empty string.
    /// </summary>
    public abstract record StringValueObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringValueObject"/> class.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <param name="name">Name of the derived value object, used in validation messages.</param>
        protected StringValueObject(string value, [CallerMemberName] string name = "")
        {
            ArgumentNullException.ThrowIfNull(value);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValueShouldNotBeEmptyException($"The {name} can not be empty.");
            }

            Value = value.Trim();
        }

        /// <summary>
        /// Gets the string value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Returns the string value as a string.
        /// </summary>
        /// <returns>The string value.</returns>
        public override string ToString()
        {
            return Value;
        }
    }
}
