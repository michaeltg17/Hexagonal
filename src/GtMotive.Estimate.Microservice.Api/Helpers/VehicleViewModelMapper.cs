using GtMotive.Estimate.Microservice.Api.ViewModels;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;

namespace GtMotive.Estimate.Microservice.Api.Helpers
{
    /// <summary>
    /// Maps the vehicle read model to its view model.
    /// </summary>
    internal static class VehicleViewModelMapper
    {
        /// <summary>
        /// Maps the vehicle read model to its view model.
        /// </summary>
        /// <param name="vehicle">Vehicle read model.</param>
        /// <returns>The vehicle view model.</returns>
        public static VehicleViewModel Map(VehicleDto vehicle)
        {
            return new VehicleViewModel
            {
                Id = vehicle.Id,
                Plate = vehicle.Plate,
                Make = vehicle.Make,
                Model = vehicle.Model,
                ManufactureDate = vehicle.ManufactureDate,
                IsAvailable = vehicle.IsAvailable,
            };
        }
    }
}
