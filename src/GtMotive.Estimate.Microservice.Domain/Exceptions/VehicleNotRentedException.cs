using System;

namespace GtMotive.Estimate.Microservice.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a vehicle is not rented.
    /// </summary>
    public class VehicleNotRentedException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleNotRentedException"/> class.
        /// </summary>
        public VehicleNotRentedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleNotRentedException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public VehicleNotRentedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleNotRentedException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public VehicleNotRentedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
