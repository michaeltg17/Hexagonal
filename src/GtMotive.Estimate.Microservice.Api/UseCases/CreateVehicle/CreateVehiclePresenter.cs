using System;
using GtMotive.Estimate.Microservice.Api.Helpers;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.CreateVehicle
{
    /// <summary>
    /// Web API presenter of the create vehicle use case.
    /// </summary>
    public sealed class CreateVehiclePresenter : IWebApiPresenter, ICreateVehicleOutputPort
    {
        private IActionResult _actionResult;

        /// <summary>
        /// Gets the action result produced by the presenter.
        /// </summary>
        public IActionResult ActionResult => _actionResult;

        /// <summary>
        /// Handles the successful creation of the vehicle.
        /// </summary>
        /// <param name="response">The output port message.</param>
        public void StandardHandle(CreateVehicleOutput response)
        {
            ArgumentNullException.ThrowIfNull(response);

            _actionResult = new ObjectResult(VehicleViewModelMapper.Map(response.Vehicle))
            {
                StatusCode = StatusCodes.Status201Created,
            };
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
