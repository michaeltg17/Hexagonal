using System.Collections.Generic;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ListVehicles
{
    /// <summary>
    /// Output message of the list vehicles use case.
    /// </summary>
    public sealed class ListVehiclesOutput : IUseCaseOutput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListVehiclesOutput"/> class.
        /// </summary>
        /// <param name="vehicles">The vehicles of the fleet.</param>
        public ListVehiclesOutput(IReadOnlyCollection<VehicleDto> vehicles)
        {
            Vehicles = vehicles;
        }

        /// <summary>
        /// Gets the vehicles of the fleet.
        /// </summary>
        public IReadOnlyCollection<VehicleDto> Vehicles { get; }
    }
}
