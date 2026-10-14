using System;

namespace GtMotive.Estimate.Microservice.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a manufacture date is in the future.
    /// </summary>
    public class ManufactureDateShouldBeInThePastException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManufactureDateShouldBeInThePastException"/> class.
        /// </summary>
        public ManufactureDateShouldBeInThePastException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManufactureDateShouldBeInThePastException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public ManufactureDateShouldBeInThePastException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManufactureDateShouldBeInThePastException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public ManufactureDateShouldBeInThePastException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
