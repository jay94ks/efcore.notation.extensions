using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Net;

namespace Efcore.Notation.Extensions.Internals.Prenotations
{
    /// <summary>
    /// Store <see cref="IPEndPoint"/> as string.
    /// </summary>
    internal class UriAsString
    {
        /// <summary>
        /// Configure the property builder.
        /// </summary>
        /// <param name="Property"></param>
        public static void Configure(PropertyBuilder Property)
        {
            if (Property is not PropertyBuilder<Uri> Uri)
                return;

            Uri.HasConversion(
                new ValueConverter<Uri, string>(X => ToString(X), X => SafeParse(X)),
                new ValueComparer<Uri>((A, B) => ToString(A) == ToString(B),
                    X => ToString(X).GetHashCode(), X => SafeParse(ToString(X))));
        }

        /// <summary>
        /// Convert <see cref="Uri"/> to string without exception.
        /// </summary>
        /// <param name="IPEndPoint"></param>
        /// <returns></returns>
        private static string ToString(Uri IPEndPoint)
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
        private static Uri SafeParse(string Input)
        {
            if (string.IsNullOrWhiteSpace(Input))
                return null;

            try { return new Uri(Input); }
            catch
            {
            }

            return null;
        }
    }
}
