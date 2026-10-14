using System.ComponentModel.DataAnnotations;

namespace GtMotive.Estimate.Microservice.Api.ViewModels
{
    /// <summary>
    /// View model to rent a vehicle.
    /// </summary>
    public class RentVehicleViewModel
    {
        /// <summary>
        /// Gets or sets the person that rents the vehicle.
        /// </summary>
        [Required]
        public string Person { get; set; }
    }
}
