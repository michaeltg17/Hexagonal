using System;
using GtMotive.Estimate.Microservice.Api.Helpers;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle
{
    /// <summary>
    /// Web API presenter of the return vehicle use case.
    /// </summary>
    public sealed class ReturnVehiclePresenter : IWebApiPresenter, IReturnVehicleOutputPort
    {
        private IActionResult _actionResult;

        /// <summary>
        /// Gets the action result produced by the presenter.
        /// </summary>
        public IActionResult ActionResult => _actionResult;

        /// <summary>
        /// Handles the successful return of the vehicle.
        /// </summary>
        /// <param name="response">The output port message.</param>
        public void StandardHandle(ReturnVehicleOutput response)
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

        /// <summary>
        /// Handles a conflict response.
        /// </summary>
        /// <param name="message">Text description.</param>
        public void ConflictHandle(string message)
        {
            _actionResult = new ConflictObjectResult(ProblemDetailsHelper.Conflict(message));
        }

        /// <summary>
        /// Handles a bad request response.
        /// </summary>
        /// <param name="message">Text description.</param>
        public void BadRequestHandle(string message)
        {
            _actionResult = new BadRequestObjectResult(ProblemDetailsHelper.BadRequest(message));
        }
    }
}
