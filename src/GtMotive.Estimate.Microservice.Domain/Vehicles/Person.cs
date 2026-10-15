using GtMotive.Estimate.Microservice.Domain.ValueObjects;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Value object that represents a person.
    /// </summary>
    public sealed record Person : StringValueObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Person"/> class.
        /// </summary>
        /// <param name="value">Person name.</param>
        public Person(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Returns the person name as a string.
        /// </summary>
        /// <returns>The person name.</returns>
        public override string ToString()
        {
            return Value;
        }
    }
}
