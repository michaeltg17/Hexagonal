using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions
{
    /// <summary>
    /// Exception thrown when a vehicle is already rented.
    /// </summary>
    public class VehicleAlreadyRentedException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleAlreadyRentedException"/> class.
        /// </summary>
        public VehicleAlreadyRentedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleAlreadyRentedException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public VehicleAlreadyRentedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleAlreadyRentedException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public VehicleAlreadyRentedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
