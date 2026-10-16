using System;

namespace GtMotive.Estimate.Microservice.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a client already has an active rental.
    /// </summary>
    public class ClientAlreadyHasActiveRentalException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientAlreadyHasActiveRentalException"/> class.
        /// </summary>
        public ClientAlreadyHasActiveRentalException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientAlreadyHasActiveRentalException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public ClientAlreadyHasActiveRentalException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientAlreadyHasActiveRentalException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public ClientAlreadyHasActiveRentalException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
