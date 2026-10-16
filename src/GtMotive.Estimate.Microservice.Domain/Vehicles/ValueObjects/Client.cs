using System.Text.RegularExpressions;
using GtMotive.Estimate.Microservice.Domain.Exceptions;
using GtMotive.Estimate.Microservice.Domain.ValueObjects;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Value object that identifies a client by its e-mail.
    /// </summary>
    public sealed partial record Client : StringValueObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="value">Client e-mail.</param>
        // CA1308: e-mail is normalized to lower case because e-mail addresses are case-insensitive.
#pragma warning disable CA1308
        public Client(string value)
            : base(value is null ? value : value.ToLowerInvariant())
#pragma warning restore CA1308
        {
            if (!EmailPattern().IsMatch(Value))
            {
                throw new ClientEmailShouldBeValidException(
                    $"The client e-mail '{Value}' is not valid.");
            }
        }

        [GeneratedRegex("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$", RegexOptions.CultureInvariant)]
        private static partial Regex EmailPattern();
    }
}
