using System.ComponentModel.DataAnnotations;

namespace GtMotive.Estimate.Microservice.Api.ViewModels
{
    /// <summary>
    /// View model to rent a vehicle.
    /// </summary>
    public sealed record RentVehicleViewModel
    {
        /// <summary>
        /// Gets or sets the e-mail that identifies the client that rents the vehicle.
        /// </summary>
        [Required]
        public string ClientEmail { get; set; }
    }
}
