using System;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Interfaces;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases
{
    /// <summary>
    /// Maps the vehicle aggregate to its read model.
    /// </summary>
    internal static class VehicleDtoMapper
    {
        /// <summary>
        /// Maps the vehicle to its read model.
        /// </summary>
        /// <param name="vehicle">Vehicle to map.</param>
        /// <returns>The vehicle read model.</returns>
        public static VehicleDto Map(IVehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            return new VehicleDto
            {
                Id = vehicle.Id.ToGuid(),
                Plate = vehicle.Plate.ToString(),
                Make = vehicle.Make.ToString(),
                Model = vehicle.Model.ToString(),
                ManufactureDate = vehicle.ManufactureDate.Value,
                IsAvailable = vehicle.IsAvailable,
            };
        }
    }
}
