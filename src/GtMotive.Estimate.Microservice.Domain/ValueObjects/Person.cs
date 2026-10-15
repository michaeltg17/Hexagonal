using System;
using GtMotive.Estimate.Microservice.Domain.Exceptions;

namespace GtMotive.Estimate.Microservice.Domain.ValueObjects
{
    /// <summary>
    /// Value object that represents a person.
    /// </summary>
    public readonly record struct Person
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Person"/> class.
        /// </summary>
        /// <param name="value">Person name.</param>
        public Person(string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValueShouldNotBeEmptyException("The person can not be empty.");
            }

            Value = value.Trim();
        }

        /// <summary>
        /// Gets the person name.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Returns the person name as a string.
        /// </summary>
        /// <returns>The person name.</returns>
        public override string ToString()
        {
            return Value;
        }
    }
}
