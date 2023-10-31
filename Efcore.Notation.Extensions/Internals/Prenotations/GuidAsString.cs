using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Efcore.Notation.Extensions.Internals.Prenotations
{
    /// <summary>
    /// Store <see cref="Guid"/> as string.
    /// </summary>
    internal class GuidAsString
    {
        /// <summary>
        /// Configure the property builder.
        /// </summary>
        /// <param name="Property"></param>
        public static void Configure(PropertyBuilder Property)
        {
            if (Property is not PropertyBuilder<Guid> GuidProperty)
                return;

            GuidProperty.HasConversion(
                new ValueConverter<Guid, string>(X => X.ToString(), X => SafeParse(X)),
                new ValueComparer<Guid>((A, B) => A == B, X => X.GetHashCode(), X => X));

            GuidProperty.HasMaxLength(16 * 2);
        }

        /// <summary>
        /// Parse the input string without exception.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        private static Guid SafeParse(string Input)
        {
            if (string.IsNullOrWhiteSpace(Input))
                return Guid.Empty;

            Guid.TryParse(Input, out var Result);
            return Result;
        }
    }
}
