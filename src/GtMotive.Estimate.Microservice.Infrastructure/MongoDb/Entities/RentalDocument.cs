using System;
using MongoDB.Bson.Serialization.Attributes;

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
        /// Gets or sets the client e-mail that identifies the client that rents the vehicle.
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        [BsonIgnoreIfNull(false)]
        public DateTime? EndDate { get; set; }
    }
}
