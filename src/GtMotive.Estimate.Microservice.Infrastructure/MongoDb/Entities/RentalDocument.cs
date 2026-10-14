using System;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Entities
{
    /// <summary>
    /// Mongo document of a rental.
    /// </summary>
    public class RentalDocument
    {
        /// <summary>
        /// Gets or sets the rental identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the vehicle identifier.
        /// </summary>
        public Guid VehicleId { get; set; }

        /// <summary>
        /// Gets or sets the person that rents the vehicle.
        /// </summary>
        public string Person { get; set; }

        /// <summary>
        /// Gets or sets the rent date.
        /// </summary>
        public DateTime? RentDate { get; set; }

        /// <summary>
        /// Gets or sets the return date.
        /// </summary>
        public DateTime? ReturnDate { get; set; }
    }
}
