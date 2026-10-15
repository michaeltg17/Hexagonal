using GtMotive.Estimate.Microservice.Domain.ValueObjects;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Value object that represents the make of a vehicle.
    /// </summary>
    public sealed record Make : StringValueObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Make"/> class.
        /// </summary>
        /// <param name="value">Make value.</param>
        public Make(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Returns the make value as a string.
        /// </summary>
        /// <returns>The make value.</returns>
        public override string ToString()
        {
            return Value;
        }
    }
}
