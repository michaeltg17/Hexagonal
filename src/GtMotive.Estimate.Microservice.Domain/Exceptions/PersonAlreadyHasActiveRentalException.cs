using System;

namespace GtMotive.Estimate.Microservice.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a person already has an active rental.
    /// </summary>
    public class PersonAlreadyHasActiveRentalException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonAlreadyHasActiveRentalException"/> class.
        /// </summary>
        public PersonAlreadyHasActiveRentalException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonAlreadyHasActiveRentalException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public PersonAlreadyHasActiveRentalException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonAlreadyHasActiveRentalException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public PersonAlreadyHasActiveRentalException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
