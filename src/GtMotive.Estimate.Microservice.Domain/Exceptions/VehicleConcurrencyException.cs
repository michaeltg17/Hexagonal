using System;

namespace GtMotive.Estimate.Microservice.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a vehicle aggregate is modified concurrently by another operation.
    /// </summary>
    public sealed class VehicleConcurrencyException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleConcurrencyException"/> class.
        /// </summary>
        public VehicleConcurrencyException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleConcurrencyException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public VehicleConcurrencyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleConcurrencyException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public VehicleConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
