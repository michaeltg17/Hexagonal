using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.Helpers
{
    /// <summary>
    /// Builds problem details responses.
    /// </summary>
    internal static class ProblemDetailsHelper
    {
        /// <summary>
        /// Builds a bad request problem details.
        /// </summary>
        /// <param name="message">Text description.</param>
        /// <returns>The problem details.</returns>
        public static ProblemDetails BadRequest(string message)
        {
            return Create(StatusCodes.Status400BadRequest, "https://tools.ietf.org/html/rfc7231#section-6.5.1", "Bad Request", message);
        }

        /// <summary>
        /// Builds a not found problem details.
        /// </summary>
        /// <param name="message">Text description.</param>
        /// <returns>The problem details.</returns>
        public static ProblemDetails NotFound(string message)
        {
            return Create(StatusCodes.Status404NotFound, "https://tools.ietf.org/html/rfc7231#section-6.5.4", "Not Found", message);
        }

        /// <summary>
        /// Builds a conflict problem details.
        /// </summary>
        /// <param name="message">Text description.</param>
        /// <returns>The problem details.</returns>
        public static ProblemDetails Conflict(string message)
        {
            return Create(StatusCodes.Status409Conflict, "https://tools.ietf.org/html/rfc7231#section-6.5.8", "Conflict", message);
        }

        private static ProblemDetails Create(int status, string type, string title, string message)
        {
            return new ProblemDetails
            {
                Type = type,
                Status = status,
                Title = title,
                Detail = message,
            };
        }
    }
}
