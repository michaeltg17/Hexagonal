using System;

namespace GtMotive.Estimate.Microservice.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a rental has already been returned.
    /// </summary>
    public class RentalAlreadyReturnedException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RentalAlreadyReturnedException"/> class.
        /// </summary>
        public RentalAlreadyReturnedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RentalAlreadyReturnedException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public RentalAlreadyReturnedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RentalAlreadyReturnedException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public RentalAlreadyReturnedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
