using System;
using GtMotive.Estimate.Microservice.Api.Helpers;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetVehicle;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.GetVehicle
{
    /// <summary>
    /// Web API presenter of the get vehicle use case.
    /// </summary>
    public sealed class GetVehiclePresenter : IWebApiPresenter, IGetVehicleOutputPort
    {
        private IActionResult _actionResult;

        /// <summary>
        /// Gets the action result produced by the presenter.
        /// </summary>
        public IActionResult ActionResult => _actionResult;

        /// <summary>
        /// Handles the successful retrieval of the vehicle.
        /// </summary>
        /// <param name="response">The output port message.</param>
        public void StandardHandle(GetVehicleOutput response)
        {
            ArgumentNullException.ThrowIfNull(response);

            _actionResult = new OkObjectResult(VehicleViewModelMapper.Map(response.Vehicle));
        }

        /// <summary>
        /// Informs the resource was not found.
        /// </summary>
        /// <param name="message">Text description.</param>
        public void NotFoundHandle(string message)
        {
            _actionResult = new NotFoundObjectResult(ProblemDetailsHelper.NotFound(message));
        }
    }
}
