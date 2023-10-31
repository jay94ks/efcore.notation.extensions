using Efcore.Notation.Extensions.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Runtime.Serialization;
using System.Text;

namespace Efcore.Notation.Extensions.Internals
{
    /// <summary>
    /// Binary Serializables.
    /// </summary>
    internal class BinarySerializables<TProperty>
    {
        /// <summary>
        /// Configure the property builder.
        /// </summary>
        /// <param name="Property"></param>
        public static void Configure(PropertyBuilder<TProperty> Property)
        {
            Property.HasConversion(
                new ValueConverter<TProperty, string>(X => ToString(X), X => SafeParse(X)),
                new ValueComparer<TProperty>((A, B) => ToString(A) == ToString(B),
                    X => ToString(X).GetHashCode(), X => SafeParse(ToString(X))));

            Property.HasColumnType("LONGTEXT");
        }

        /// <summary>
        /// Convert <see cref="Uri"/> to string without exception.
        /// </summary>
        /// <param name="Serializable"></param>
        /// <returns></returns>
        private static string ToString(TProperty Serializable)
        {
            if (Serializable is null)
                return string.Empty;

            try
            {
                using var Stream = new MemoryStream();
                using (var Writer = new BinaryWriter(Stream, Encoding.UTF8, true))
                {
                    if (Serializable is IBinarySerializable Binary)
                        Binary.Serialize(Writer);
                }

                return Convert.ToBase64String(Stream.ToArray(), Base64FormattingOptions.None);
            }

            catch { }
            return string.Empty;
        }

        /// <summary>
        /// Parse the input string without exception.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        private static TProperty SafeParse(string Input)
        {
            if (string.IsNullOrWhiteSpace(Input))
                return default;

            try
            {
                var Data = Convert.FromBase64String(Input);
                if (Data is null)
                    return default;

                var Ctor = typeof(TProperty).GetConstructor(Type.EmptyTypes);
                if (Ctor is null)
                    return default;

                using var Stream = new MemoryStream(Data, false);
                using (var Reader = new BinaryReader(Stream, Encoding.UTF8, true))
                {
                    var Result = (TProperty)Ctor.Invoke(Array.Empty<object>());
                    if (Result is IBinarySerializable Binary)
                        Binary.Deserialize(Reader);

                    return Result;
                }
            }

            catch { }
            return default;
        }
    }
}
