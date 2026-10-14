using System;

namespace GtMotive.Estimate.Microservice.Api.ViewModels
{
    /// <summary>
    /// View model of a vehicle.
    /// </summary>
    public class VehicleViewModel
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
        /// Gets or sets a value indicating whether the vehicle is available.
        /// </summary>
        public bool IsAvailable { get; set; }
    }
}
