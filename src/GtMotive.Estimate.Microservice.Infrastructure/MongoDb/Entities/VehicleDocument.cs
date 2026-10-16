using System;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Entities
{
    /// <summary>
    /// Mongo document of the vehicle aggregate.
    /// </summary>
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
        /// Gets or sets the identifier of the active rental, if the vehicle is rented.
        /// </summary>
        public Guid? ActiveRentalId { get; set; }

        /// <summary>
        /// Gets or sets the optimistic concurrency version of the document.
        /// </summary>
        public long Version { get; set; }
    }
}
