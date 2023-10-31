using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Net;

namespace Efcore.Notation.Extensions.Internals.Prenotations
{
    /// <summary>
    /// Store <see cref="IPEndPoint"/> as string.
    /// </summary>
    internal class IPEndPointAsString
    {
        /// <summary>
        /// Max Length in textual expression.
        /// 4 letters are for safety.
        /// 45 (IPv6 MAX) + 7 (IPv6 Brackets with Port number)
        /// </summary>
        public const int MaxLength = 48 + 8;

        // eg. 000:0000:0000:0000:0000:0000:0000:0000               39
        // eg. 0000:0000:0000:0000:0000:ffff:192.168.100.228        45
        // port: 0 ~ 65536 (5), brackets (IPv6): 2.                 8

        /// <summary>
        /// Configure the property builder.
        /// </summary>
        /// <param name="Property"></param>
        public static void Configure(PropertyBuilder Property)
        {
            if (Property is not PropertyBuilder<IPEndPoint> IPEndPoint)
                return;

            IPEndPoint.HasConversion(
                new ValueConverter<IPEndPoint, string>(X => ToString(X), X => SafeParse(X)),
                new ValueComparer<IPEndPoint>((A, B) => ToString(A) == ToString(B),
                    X => ToString(X).GetHashCode(), X => SafeParse(ToString(X))));

            IPEndPoint.HasMaxLength(MaxLength);
        }

        /// <summary>
        /// Convert <see cref="IPEndPoint"/> to string without exception.
        /// </summary>
        /// <param name="IPEndPoint"></param>
        /// <returns></returns>
        private static string ToString(IPEndPoint IPEndPoint)
        {
            if (IPEndPoint is null)
                return string.Empty;

            return IPEndPoint.ToString();
        }

        /// <summary>
        /// Parse the input string without exception.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        private static IPEndPoint SafeParse(string Input)
        {
            if (string.IsNullOrWhiteSpace(Input))
                return null;

            IPEndPoint.TryParse(Input, out var Result);
            return Result;
        }
    }
}
