using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions
{
    /// <summary>
    /// Exception thrown when a vehicle is older than the maximum allowed age.
    /// </summary>
    public class VehicleShouldNotBeOlderThanFiveYearsException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleShouldNotBeOlderThanFiveYearsException"/> class.
        /// </summary>
        public VehicleShouldNotBeOlderThanFiveYearsException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleShouldNotBeOlderThanFiveYearsException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public VehicleShouldNotBeOlderThanFiveYearsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleShouldNotBeOlderThanFiveYearsException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public VehicleShouldNotBeOlderThanFiveYearsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
