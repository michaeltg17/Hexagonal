using System;

namespace GtMotive.Estimate.Microservice.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a vehicle can not be found.
    /// </summary>
    public class VehicleNotFoundException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleNotFoundException"/> class.
        /// </summary>
        public VehicleNotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public VehicleNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public VehicleNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
