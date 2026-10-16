using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions
{
    /// <summary>
    /// Exception thrown when a client e-mail is not valid.
    /// </summary>
    public class ClientEmailShouldBeValidException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientEmailShouldBeValidException"/> class.
        /// </summary>
        public ClientEmailShouldBeValidException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientEmailShouldBeValidException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public ClientEmailShouldBeValidException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientEmailShouldBeValidException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public ClientEmailShouldBeValidException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
