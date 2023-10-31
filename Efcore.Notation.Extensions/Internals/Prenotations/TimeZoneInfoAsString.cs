using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efcore.Notation.Extensions.Internals.Prenotations
{
    /// <summary>
    /// Store <see cref="TimeZoneInfo"/> as string.
    /// </summary>
    internal class TimeZoneInfoAsString
    {
        /// <summary>
        /// Configure the property builder.
        /// </summary>
        /// <param name="Property"></param>
        public static void Configure(PropertyBuilder Property)
        {
            if (Property is not PropertyBuilder<TimeZoneInfo> Encoding)
                return;

            Encoding.HasConversion(
                new ValueConverter<TimeZoneInfo, string>(X => ToString(X), X => SafeParse(X)),
                new ValueComparer<TimeZoneInfo>((A, B) => ToString(A) == ToString(B),
                    X => ToString(X).GetHashCode(), X => SafeParse(ToString(X))));

            Encoding.HasMaxLength(48);
        }

        /// <summary>
        /// Convert <see cref="Encoding"/> to string without exception.
        /// </summary>
        /// <param name="Encoding"></param>
        /// <returns></returns>
        private static string ToString(TimeZoneInfo Encoding)
        {
            if (Encoding is null)
                return string.Empty;

            return Encoding.Id;
        }

        /// <summary>
        /// Parse the input string without exception.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        private static TimeZoneInfo SafeParse(string Input)
        {
            if (string.IsNullOrWhiteSpace(Input))
                return null;

            try { return TimeZoneInfo.FindSystemTimeZoneById(Input); }
            catch
            {
            }

            return null;
        }
    }
}
