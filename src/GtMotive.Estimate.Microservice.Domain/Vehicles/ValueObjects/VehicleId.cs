using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles.ValueObjects
{
    /// <summary>
    /// Value object that represents the identifier of a vehicle.
    /// </summary>
    public readonly record struct VehicleId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleId"/> class.
        /// </summary>
        /// <param name="value">Vehicle identifier value.</param>
        public VehicleId(Guid value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the vehicle identifier value.
        /// </summary>
        public Guid Value { get; }

        /// <summary>
        /// Gets the vehicle identifier as a <see cref="Guid"/>.
        /// </summary>
        /// <returns>The vehicle identifier.</returns>
        public Guid ToGuid()
        {
            return Value;
        }

        /// <summary>
        /// Returns the vehicle identifier as a string.
        /// </summary>
        /// <returns>The vehicle identifier.</returns>
        public override string ToString()
        {
            return Value.ToString("D");
        }
    }
}
