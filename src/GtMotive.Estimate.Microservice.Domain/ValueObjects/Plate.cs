using System;
using GtMotive.Estimate.Microservice.Domain.Exceptions;

namespace GtMotive.Estimate.Microservice.Domain.ValueObjects
{
    /// <summary>
    /// Value object that represents the plate of a vehicle.
    /// </summary>
    public readonly struct Plate : IEquatable<Plate>
    {
        private readonly string _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Plate"/> struct.
        /// </summary>
        /// <param name="value">Plate value.</param>
        public Plate(string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValueShouldNotBeEmptyException("The plate can not be empty.");
            }

            _value = value.Trim();
        }

        /// <summary>
        /// Gets the plate value.
        /// </summary>
        public string Value => _value;

        /// <summary>
        /// Determines whether the specified <see cref="Plate"/> instances are equal.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>Whether the operands are equal.</returns>
        public static bool operator ==(Plate left, Plate right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Plate"/> instances are different.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>Whether the operands are different.</returns>
        public static bool operator !=(Plate left, Plate right)
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
            return obj is Plate other && this.Equals(other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Plate"/> is equal to the current one.
        /// </summary>
        /// <param name="other">Plate to compare.</param>
        /// <returns>Whether the plates are equal.</returns>
        public bool Equals(Plate other)
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
        /// Returns the plate value as a string.
        /// </summary>
        /// <returns>The plate value.</returns>
        public override string ToString()
        {
            return _value;
        }
    }
}
