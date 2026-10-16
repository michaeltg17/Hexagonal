using System;
using GtMotive.Estimate.Microservice.Domain.Exceptions;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Value object that represents the manufacture date of a vehicle.
    /// </summary>
    public readonly record struct ManufactureDate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManufactureDate"/> class.
        /// </summary>
        /// <param name="value">Manufacture date value.</param>
        public ManufactureDate(DateTime value)
        {
            if (value > DateTime.UtcNow)
            {
                throw new ManufactureDateShouldBeInThePastException(
                    "The manufacture date can not be in the future.");
            }

            if (value.AddYears(5) < DateTime.UtcNow)
            {
                throw new VehicleShouldNotBeOlderThanFiveYearsException(
                    "The vehicle is older than the maximum allowed age of 5 years.");
            }

            Value = value;
        }

        /// <summary>
        /// Gets the manufacture date value.
        /// </summary>
        public DateTime Value { get; }

        /// <summary>
        /// Returns the manufacture date as a string.
        /// </summary>
        /// <returns>The manufacture date in round-trip format.</returns>
        public override string ToString()
        {
            return Value.ToString("O");
        }
    }
}
