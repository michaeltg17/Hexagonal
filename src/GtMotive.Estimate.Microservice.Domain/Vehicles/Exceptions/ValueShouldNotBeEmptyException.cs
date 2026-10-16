using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions
{
    /// <summary>
    /// Exception thrown when a value should not be empty.
    /// </summary>
    public class ValueShouldNotBeEmptyException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueShouldNotBeEmptyException"/> class.
        /// </summary>
        public ValueShouldNotBeEmptyException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueShouldNotBeEmptyException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public ValueShouldNotBeEmptyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueShouldNotBeEmptyException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public ValueShouldNotBeEmptyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
