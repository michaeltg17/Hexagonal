using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Value object that represents the identifier of a rental.
    /// </summary>
    public readonly record struct RentalId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RentalId"/> class.
        /// </summary>
        /// <param name="value">Rental identifier value.</param>
        public RentalId(Guid value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the rental identifier value.
        /// </summary>
        public Guid Value { get; }

        /// <summary>
        /// Gets the rental identifier as a <see cref="Guid"/>.
        /// </summary>
        /// <returns>The rental identifier.</returns>
        public Guid ToGuid()
        {
            return Value;
        }

        /// <summary>
        /// Returns the rental identifier as a string.
        /// </summary>
        /// <returns>The rental identifier.</returns>
        public override string ToString()
        {
            return Value.ToString("D");
        }
    }
}
