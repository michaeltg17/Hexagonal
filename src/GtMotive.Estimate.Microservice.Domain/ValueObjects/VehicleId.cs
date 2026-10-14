using System;

namespace GtMotive.Estimate.Microservice.Domain.ValueObjects
{
    /// <summary>
    /// Value object that represents the identifier of a vehicle.
    /// </summary>
    public readonly struct VehicleId : IEquatable<VehicleId>
    {
        private readonly Guid _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleId"/> struct.
        /// </summary>
        /// <param name="value">Vehicle identifier value.</param>
        public VehicleId(Guid value)
        {
            _value = value;
        }

        /// <summary>
        /// Determines whether the specified <see cref="VehicleId"/> instances are equal.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>Whether the operands are equal.</returns>
        public static bool operator ==(VehicleId left, VehicleId right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether the specified <see cref="VehicleId"/> instances are different.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>Whether the operands are different.</returns>
        public static bool operator !=(VehicleId left, VehicleId right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Gets the vehicle identifier as a <see cref="Guid"/>.
        /// </summary>
        /// <returns>The vehicle identifier.</returns>
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
            return obj is VehicleId other && Equals(other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="VehicleId"/> is equal to the current one.
        /// </summary>
        /// <param name="other">Vehicle identifier to compare.</param>
        /// <returns>Whether the identifiers are equal.</returns>
        public bool Equals(VehicleId other)
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
        /// Returns the vehicle identifier as a string.
        /// </summary>
        /// <returns>The vehicle identifier.</returns>
        public override string ToString()
        {
            return _value.ToString("D");
        }
    }
}
