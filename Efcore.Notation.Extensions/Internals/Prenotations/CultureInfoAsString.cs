using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Globalization;
using System.Text;

namespace Efcore.Notation.Extensions.Internals.Prenotations
{
    /// <summary>
    /// Store <see cref="CultureInfo"/> as string.
    /// </summary>
    internal class CultureInfoAsString
    {
        /// <summary>
        /// Configure the property builder.
        /// </summary>
        /// <param name="Property"></param>
        public static void Configure(PropertyBuilder Property)
        {
            if (Property is not PropertyBuilder<CultureInfo> Encoding)
                return;

            Encoding.HasConversion(
                new ValueConverter<CultureInfo, string>(X => ToString(X), X => SafeParse(X)),
                new ValueComparer<CultureInfo>((A, B) => ToString(A) == ToString(B),
                    X => ToString(X).GetHashCode(), X => SafeParse(ToString(X))));

            Encoding.HasMaxLength(16);
        }

        /// <summary>
        /// Convert <see cref="Encoding"/> to string without exception.
        /// </summary>
        /// <param name="Encoding"></param>
        /// <returns></returns>
        private static string ToString(CultureInfo Encoding)
        {
            if (Encoding is null)
                return string.Empty;

            return Encoding.Name;
        }

        /// <summary>
        /// Parse the input string without exception.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        private static CultureInfo SafeParse(string Input)
        {
            if (string.IsNullOrWhiteSpace(Input))
                return null;

            try { return CultureInfo.GetCultureInfo(Input); }
            catch
            {
            }

            return null;
        }
    }
}
