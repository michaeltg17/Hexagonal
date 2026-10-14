using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Entities
{
    /// <summary>
    /// Mongo document of the vehicle aggregate.
    /// </summary>
    [SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Bson documents map naturally to a list of rentals.")]
    [SuppressMessage("Design", "CA2227:Collection properties should be read only", Justification = "Bson deserialization requires a settable collection.")]
    public class VehicleDocument
    {
        /// <summary>
        /// Gets or sets the vehicle identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the plate.
        /// </summary>
        public string Plate { get; set; }

        /// <summary>
        /// Gets or sets the make.
        /// </summary>
        public string Make { get; set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the manufacture date.
        /// </summary>
        public DateTime ManufactureDate { get; set; }

        /// <summary>
        /// Gets or sets the rentals of the vehicle.
        /// </summary>
        public List<RentalDocument> Rentals { get; set; } = [];
    }
}
