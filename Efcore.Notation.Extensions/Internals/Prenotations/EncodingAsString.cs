using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text;
using System.Text.RegularExpressions;

namespace Efcore.Notation.Extensions.Internals.Prenotations
{
    internal class EncodingAsString
    {
        /// <summary>
        /// Enable code-page supports for <see cref="Encoding"/>.
        /// </summary>
        static EncodingAsString()
        {
            try { Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); }
            catch
            {
            }
        }

        /// <summary>
        /// Configure the property builder.
        /// </summary>
        /// <param name="Property"></param>
        public static void Configure(PropertyBuilder Property)
        {
            if (Property is not PropertyBuilder<Encoding> Encoding)
                return;

            Encoding.HasConversion(
                new ValueConverter<Encoding, string>(X => ToString(X), X => SafeParse(X)),
                new ValueComparer<Encoding>((A, B) => ToString(A) == ToString(B),
                    X => ToString(X).GetHashCode(), X => SafeParse(ToString(X))));

            Encoding.HasMaxLength(12);
        }

        /// <summary>
        /// Convert <see cref="Encoding"/> to string without exception.
        /// </summary>
        /// <param name="Encoding"></param>
        /// <returns></returns>
        private static string ToString(Encoding Encoding)
        {
            if (Encoding is null)
                return string.Empty;

            return Encoding.WebName;
        }

        /// <summary>
        /// Parse the input string without exception.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        private static Encoding SafeParse(string Input)
        {
            if (string.IsNullOrWhiteSpace(Input))
                return null;

            try { return Encoding.GetEncoding(Input); }
            catch
            {
            }

            return null;
        }
    }
}
