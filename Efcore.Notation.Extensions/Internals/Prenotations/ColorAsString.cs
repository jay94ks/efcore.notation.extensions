using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Drawing;
using System.Net;

namespace Efcore.Notation.Extensions.Internals.Prenotations
{
    /// <summary>
    /// Store <see cref="Color"/> as string.
    /// </summary>
    internal class ColorAsString
    {
        /// <summary>
        /// # + 123456 + 78 (optional)
        /// </summary>
        public const int MaxLength = 9;

        /// <summary>
        /// Configure the property builder.
        /// </summary>
        /// <param name="Property"></param>
        public static void Configure(PropertyBuilder Property)
        {
            if (Property is not PropertyBuilder<Color> Color)
                return;

            Color.HasConversion(
                new ValueConverter<Color, string>(X => ToString(X), X => SafeParse(X)),
                new ValueComparer<Color>((A, B) => ToString(A) == ToString(B),
                    X => ToString(X).GetHashCode(), X => SafeParse(ToString(X))));

            Color.HasMaxLength(MaxLength);
        }

        /// <summary>
        /// Convert <see cref="Color"/> to string without exception.
        /// </summary>
        /// <param name="IPEndPoint"></param>
        /// <returns></returns>
        private static string ToString(Color IPEndPoint)
        {
            Span<byte> Temp = stackalloc byte[4];

            Temp[0] = IPEndPoint.R;
            Temp[1] = IPEndPoint.G;
            Temp[2] = IPEndPoint.B;

            if (IPEndPoint.A != 255)
            {
                Temp[3] = IPEndPoint.A;
                return "#" + Convert.ToHexString(Temp).ToLower();
            }

            return "#" + Convert.ToHexString(Temp.Slice(0, 3)).ToLower();
        }

        /// <summary>
        /// Parse the input string without exception.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        private static Color SafeParse(string Input)
        {
            if (string.IsNullOrWhiteSpace(Input))
                return Color.Transparent;

            if (Input.StartsWith('#') == false)
                return Color.Transparent;

            Input = Input.Substring(1);
            while ((Input.Length % 2) != 0)
                Input += "0";

            try
            {
                var Bytes = Convert.FromHexString(Input);
                if (Bytes.Length <= 3)
                {
                    var Last = Bytes.LastOrDefault();
                    if (Bytes.Length != 3)
                    {
                        var Org = Bytes.Length;
                        Array.Resize(ref Bytes, 3);
                        Array.Fill(Bytes, Last, Org, Bytes.Length - Org);
                    }

                    return Color.FromArgb(255, Bytes[0], Bytes[1], Bytes[2]);
                }

                return Color.FromArgb(Bytes[3], Bytes[0], Bytes[1], Bytes[2]);
            }

            catch { }
            return Color.Transparent;
        }
    }
}
