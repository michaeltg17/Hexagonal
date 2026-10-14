using System;
using System.Linq;
using GtMotive.Estimate.Microservice.Api.Helpers;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ListAvailableVehicles;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.ListAvailableVehicles
{
    /// <summary>
    /// Web API presenter of the list available vehicles use case.
    /// </summary>
    public sealed class ListAvailableVehiclesPresenter : IWebApiPresenter, IListAvailableVehiclesOutputPort
    {
        private IActionResult _actionResult;

        /// <summary>
        /// Gets the action result produced by the presenter.
        /// </summary>
        public IActionResult ActionResult => _actionResult;

        /// <summary>
        /// Handles the successful listing of the vehicles.
        /// </summary>
        /// <param name="response">The output port message.</param>
        public void StandardHandle(ListAvailableVehiclesOutput response)
        {
            ArgumentNullException.ThrowIfNull(response);

            var vehicles = response.Vehicles.Select(VehicleViewModelMapper.Map).ToList();

            _actionResult = new OkObjectResult(vehicles);
        }
    }
}
