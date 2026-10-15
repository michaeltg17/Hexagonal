namespace GtMotive.Estimate.Microservice.Domain.ValueObjects
{
    /// <summary>
    /// Value object that represents the plate of a vehicle.
    /// </summary>
    public sealed record Plate : StringValueObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Plate"/> class.
        /// </summary>
        /// <param name="value">Plate value.</param>
        public Plate(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Returns the plate value as a string.
        /// </summary>
        /// <returns>The plate value.</returns>
        public override string ToString()
        {
            return Value;
        }
    }
}
