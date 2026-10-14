using System;

namespace GtMotive.Estimate.Microservice.Domain.ValueObjects
{
    /// <summary>
    /// Value object that represents the identifier of a rental.
    /// </summary>
    public readonly struct RentalId : IEquatable<RentalId>
    {
        private readonly Guid _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="RentalId"/> struct.
        /// </summary>
        /// <param name="value">Rental identifier value.</param>
        public RentalId(Guid value)
        {
            _value = value;
        }

        /// <summary>
        /// Determines whether the specified <see cref="RentalId"/> instances are equal.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>Whether the operands are equal.</returns>
        public static bool operator ==(RentalId left, RentalId right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether the specified <see cref="RentalId"/> instances are different.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>Whether the operands are different.</returns>
        public static bool operator !=(RentalId left, RentalId right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Gets the rental identifier as a <see cref="Guid"/>.
        /// </summary>
        /// <returns>The rental identifier.</returns>
        public Guid ToGuid()
        {
            return _value;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current one.
        /// </summary>
        /// <param name="obj">Object to compare.</param>
        /// <returns>Whether the objects are equal.</returns>
        public override bool Equals(object obj)
        {
            return obj is RentalId other && this.Equals(other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="RentalId"/> is equal to the current one.
        /// </summary>
        /// <param name="other">Rental identifier to compare.</param>
        /// <returns>Whether the identifiers are equal.</returns>
        public bool Equals(RentalId other)
        {
            return _value == other._value;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        /// <summary>
        /// Returns the rental identifier as a string.
        /// </summary>
        /// <returns>The rental identifier.</returns>
        public override string ToString()
        {
            return _value.ToString("D");
        }
    }
}
