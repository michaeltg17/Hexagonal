using GtMotive.Estimate.Microservice.Domain.ValueObjects;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Value object that represents the model of a vehicle.
    /// </summary>
    public sealed record Model : StringValueObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> class.
        /// </summary>
        /// <param name="value">Model value.</param>
        public Model(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Returns the model value as a string.
        /// </summary>
        /// <returns>The model value.</returns>
        public override string ToString()
        {
            return Value;
        }
    }
}
