using System.Collections.Generic;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ListAvailableVehicles
{
    /// <summary>
    /// Output message of the list available vehicles use case.
    /// </summary>
    public sealed class ListAvailableVehiclesOutput : IUseCaseOutput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListAvailableVehiclesOutput"/> class.
        /// </summary>
        /// <param name="vehicles">The vehicles of the fleet.</param>
        public ListAvailableVehiclesOutput(IReadOnlyCollection<VehicleDto> vehicles)
        {
            Vehicles = vehicles;
        }

        /// <summary>
        /// Gets the vehicles of the fleet.
        /// </summary>
        public IReadOnlyCollection<VehicleDto> Vehicles { get; }
    }
}
