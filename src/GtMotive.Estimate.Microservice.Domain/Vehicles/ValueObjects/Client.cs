using System;
using System.Text.RegularExpressions;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles.ValueObjects
{
    /// <summary>
    /// Value object that identifies a client by its e-mail.
    /// </summary>
    public sealed partial record Client
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="value">Client e-mail.</param>
        public Client(string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValueShouldNotBeEmptyException("The client e-mail can not be empty.");
            }

            // CA1308: e-mail is normalized to lower case because e-mail addresses are case-insensitive.
#pragma warning disable CA1308
            var email = value.Trim().ToLowerInvariant();
#pragma warning restore CA1308

            if (!EmailPattern().IsMatch(email))
            {
                throw new ClientEmailShouldBeValidException(
                    $"The client e-mail '{value}' is not valid.");
            }

            Value = email;
        }

        /// <summary>
        /// Gets the client e-mail.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Returns the client e-mail as a string.
        /// </summary>
        /// <returns>The client e-mail.</returns>
        public override string ToString()
        {
            return Value;
        }

        [GeneratedRegex("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$", RegexOptions.CultureInvariant)]
        private static partial Regex EmailPattern();
    }
}
