using System;
using GtMotive.Estimate.Microservice.Domain.Exceptions;

namespace GtMotive.Estimate.Microservice.Domain.ValueObjects
{
    /// <summary>
    /// Value object that represents the model of a vehicle.
    /// </summary>
    public readonly struct Model : IEquatable<Model>
    {
        private readonly string _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> struct.
        /// </summary>
        /// <param name="value">Model value.</param>
        public Model(string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValueShouldNotBeEmptyException("The model can not be empty.");
            }

            _value = value.Trim();
        }

        /// <summary>
        /// Gets the model value.
        /// </summary>
        public string Value => _value;

        /// <summary>
        /// Determines whether the specified <see cref="Model"/> instances are equal.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>Whether the operands are equal.</returns>
        public static bool operator ==(Model left, Model right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Model"/> instances are different.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>Whether the operands are different.</returns>
        public static bool operator !=(Model left, Model right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current one.
        /// </summary>
        /// <param name="obj">Object to compare.</param>
        /// <returns>Whether the objects are equal.</returns>
        public override bool Equals(object obj)
        {
            return obj is Model other && this.Equals(other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Model"/> is equal to the current one.
        /// </summary>
        /// <param name="other">Model to compare.</param>
        /// <returns>Whether the models are equal.</returns>
        public bool Equals(Model other)
        {
            return _value == other._value;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return _value.GetHashCode(StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns the model value as a string.
        /// </summary>
        /// <returns>The model value.</returns>
        public override string ToString()
        {
            return _value;
        }
    }
}
