using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Net;

namespace Efcore.Notation.Extensions.Internals.Prenotations
{
    /// <summary>
    /// Store <see cref="IPAddress"/> as string.
    /// </summary>
    internal class IPAddressAsString
    {
        /// <summary>
        /// Max Length in textual expression.
        /// 3 letters are for safety.
        /// </summary>
        public const int MaxLength = 48;

        // eg. 000:0000:0000:0000:0000:0000:0000:0000               39
        // eg. 0000:0000:0000:0000:0000:ffff:192.168.100.228        45

        /// <summary>
        /// Configure the property builder.
        /// </summary>
        /// <param name="Property"></param>
        public static void Configure(PropertyBuilder Property)
        {
            if (Property is not PropertyBuilder<IPAddress> IPAddress)
                return;

            IPAddress.HasConversion(
                new ValueConverter<IPAddress, string>(X => ToString(X), X => SafeParse(X)),
                new ValueComparer<IPAddress>((A, B) => ToString(A) == ToString(B),
                    X => ToString(X).GetHashCode(), X => SafeParse(ToString(X))));

            IPAddress.HasMaxLength(MaxLength);
        }

        /// <summary>
        /// Convert <see cref="IPAddress"/> to string without exception.
        /// </summary>
        /// <param name="IPAddress"></param>
        /// <returns></returns>
        private static string ToString(IPAddress IPAddress)
        {
            if (IPAddress is null)
                return string.Empty;

            return IPAddress.ToString();
        }

        /// <summary>
        /// Parse the input string without exception.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        private static IPAddress SafeParse(string Input)
        {
            if (string.IsNullOrWhiteSpace(Input))
                return null;

            IPAddress.TryParse(Input, out var Result);
            return Result;
        }
    }
}
