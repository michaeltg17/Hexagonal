using System;

namespace GtMotive.Estimate.Microservice.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a vehicle already exists in the fleet.
    /// </summary>
    public class VehicleAlreadyExistsException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleAlreadyExistsException"/> class.
        /// </summary>
        public VehicleAlreadyExistsException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public VehicleAlreadyExistsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public VehicleAlreadyExistsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
