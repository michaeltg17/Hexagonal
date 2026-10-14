using System;
using GtMotive.Estimate.Microservice.Domain.Exceptions;

namespace GtMotive.Estimate.Microservice.Domain.ValueObjects
{
    /// <summary>
    /// Value object that represents the make of a vehicle.
    /// </summary>
    public readonly struct Make : IEquatable<Make>
    {
        private readonly string _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Make"/> struct.
        /// </summary>
        /// <param name="value">Make value.</param>
        public Make(string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValueShouldNotBeEmptyException("The make can not be empty.");
            }

            _value = value.Trim();
        }

        /// <summary>
        /// Gets the make value.
        /// </summary>
        public string Value => _value;

        /// <summary>
        /// Determines whether the specified <see cref="Make"/> instances are equal.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>Whether the operands are equal.</returns>
        public static bool operator ==(Make left, Make right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Make"/> instances are different.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>Whether the operands are different.</returns>
        public static bool operator !=(Make left, Make right)
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
            return obj is Make other && this.Equals(other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Make"/> is equal to the current one.
        /// </summary>
        /// <param name="other">Make to compare.</param>
        /// <returns>Whether the makes are equal.</returns>
        public bool Equals(Make other)
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
        /// Returns the make value as a string.
        /// </summary>
        /// <returns>The make value.</returns>
        public override string ToString()
        {
            return _value;
        }
    }
}
