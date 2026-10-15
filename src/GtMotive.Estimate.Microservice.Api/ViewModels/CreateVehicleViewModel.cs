using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GtMotive.Estimate.Microservice.Api.ViewModels
{
    /// <summary>
    /// View model to create a new vehicle.
    /// </summary>
    public sealed record CreateVehicleViewModel
    {
        /// <summary>
        /// Gets or sets the plate.
        /// </summary>
        [Required]
        public string Plate { get; set; }

        /// <summary>
        /// Gets or sets the make.
        /// </summary>
        [Required]
        public string Make { get; set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        [Required]
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the manufacture date.
        /// </summary>
        [Required]
        [JsonRequired]
        public DateTime ManufactureDate { get; set; }
    }
}
