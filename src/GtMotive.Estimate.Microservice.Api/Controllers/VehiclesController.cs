using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Api.UseCases.CreateVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.GetVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.ListVehicles;
using GtMotive.Estimate.Microservice.Api.UseCases.RentVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle;
using GtMotive.Estimate.Microservice.Api.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.Controllers
{
    /// <summary>
    /// Vehicles controller.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="VehiclesController"/> class.
    /// </remarks>
    /// <param name="requestSender">Request sender.</param>
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController(ISender requestSender) : ControllerBase
    {
        private readonly ISender _requestSender = requestSender ?? throw new ArgumentNullException(nameof(requestSender));

        /// <summary>
        /// Creates a new vehicle in the fleet.
        /// </summary>
        /// <param name="vehicle">Vehicle to create.</param>
        /// <returns>The created vehicle.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(VehicleViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateVehicleViewModel vehicle)
        {
            var presenter = await _requestSender.Send(new CreateVehicleRequest(vehicle));

            return presenter.ActionResult;
        }

        /// <summary>
        /// Lists the vehicles of the fleet.
        /// </summary>
        /// <param name="available">Availability filter: <c>null</c> all, <c>true</c> only available, <c>false</c> only unavailable.</param>
        /// <returns>The vehicles of the fleet.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<VehicleViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromQuery] bool? available)
        {
            var presenter = await _requestSender.Send(new ListVehiclesRequest(available));

            return presenter.ActionResult;
        }

        /// <summary>
        /// Gets a vehicle of the fleet by its identifier.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <returns>The vehicle.</returns>
        [HttpGet("{vehicleId:guid}")]
        [ProducesResponseType(typeof(VehicleViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid vehicleId)
        {
            var presenter = await _requestSender.Send(new GetVehicleRequest(vehicleId));

            return presenter.ActionResult;
        }

        /// <summary>
        /// Rents a vehicle to a person.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <param name="rental">Rental data.</param>
        /// <returns>The rented vehicle.</returns>
        [HttpPost("{vehicleId:guid}/rent")]
        [ProducesResponseType(typeof(VehicleViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Rent(Guid vehicleId, [FromBody] RentVehicleViewModel rental)
        {
            var presenter = await _requestSender.Send(new RentVehicleRequest(vehicleId, rental));

            return presenter.ActionResult;
        }

        /// <summary>
        /// Returns a rented vehicle.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <returns>The returned vehicle.</returns>
        [HttpPost("{vehicleId:guid}/return")]
        [ProducesResponseType(typeof(VehicleViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ReturnVehicle(Guid vehicleId)
        {
            var presenter = await _requestSender.Send(new ReturnVehicleRequest(vehicleId));

            return presenter.ActionResult;
        }
    }
}
