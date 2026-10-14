using System;
using GtMotive.Estimate.Microservice.Domain.Exceptions;

namespace GtMotive.Estimate.Microservice.Domain.ValueObjects
{
    /// <summary>
    /// Value object that represents the manufacture date of a vehicle.
    /// </summary>
    public readonly struct ManufactureDate : IEquatable<ManufactureDate>
    {
        private readonly DateTime _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManufactureDate"/> struct.
        /// </summary>
        /// <param name="value">Manufacture date value.</param>
        public ManufactureDate(DateTime value)
        {
            if (value > DateTime.UtcNow)
            {
                throw new ManufactureDateShouldBeInThePastException(
                    "The manufacture date can not be in the future.");
            }

            if (value.AddYears(5) < DateTime.UtcNow)
            {
                throw new VehicleShouldNotBeOlderThanFiveYearsException(
                    "The vehicle is older than the maximum allowed age of 5 years.");
            }

            _value = value;
        }

        /// <summary>
        /// Gets the manufacture date value.
        /// </summary>
        public DateTime Value => _value;

        /// <summary>
        /// Determines whether the specified <see cref="ManufactureDate"/> instances are equal.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>Whether the operands are equal.</returns>
        public static bool operator ==(ManufactureDate left, ManufactureDate right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether the specified <see cref="ManufactureDate"/> instances are different.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>Whether the operands are different.</returns>
        public static bool operator !=(ManufactureDate left, ManufactureDate right)
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
            return obj is ManufactureDate other && this.Equals(other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="ManufactureDate"/> is equal to the current one.
        /// </summary>
        /// <param name="other">Manufacture date to compare.</param>
        /// <returns>Whether the manufacture dates are equal.</returns>
        public bool Equals(ManufactureDate other)
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
        /// Returns the manufacture date as a string.
        /// </summary>
        /// <returns>The manufacture date in round-trip format.</returns>
        public override string ToString()
        {
            return _value.ToString("O");
        }
    }
}
