using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Efcore.Notation.Extensions.Internals.Prenotations
{
    /// <summary>
    /// Store <see cref="JObject"/>, <see cref="JArray"/>, <see cref="JToken"/> as string.
    /// </summary>
    internal class JsonAsString
    {
        /// <summary>
        /// Configure the property builder.
        /// </summary>
        /// <param name="Property"></param>
        public static void Configure(PropertyBuilder Property)
        {
            switch (Property)
            {
                case PropertyBuilder<JObject> Jobj:
                    {
                        Jobj.HasConversion(
                            new ValueConverter<JObject, string>(X => Stringify(X), X => Parse<JObject>(X)),
                            new ValueComparer<JObject>((A, B) => Stringify(A) == Stringify(B),
                                X => Stringify(X).GetHashCode(), X => Parse<JObject>(Stringify(X))));

                        Property.HasColumnType("LONGTEXT");
                    }
                    break;

                case PropertyBuilder<JArray> Jarr:
                    {
                        Jarr.HasConversion(
                            new ValueConverter<JArray, string>(X => Stringify(X), X => Parse<JArray>(X)),
                            new ValueComparer<JArray>((A, B) => Stringify(A) == Stringify(B),
                                X => Stringify(X).GetHashCode(), X => Parse<JArray>(Stringify(X))));

                        Property.HasColumnType("LONGTEXT");
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Stringify <see cref="JToken"/>.
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        private static string Stringify(JToken Token)
        {
            if (Token is null)
                return string.Empty;

            return Token.ToString(Formatting.None);
        }

        /// <summary>
        /// Parse the input string to <typeparamref name="TJson"/>.
        /// </summary>
        /// <typeparam name="TJson"></typeparam>
        /// <param name="Input"></param>
        /// <returns></returns>
        private static TJson Parse<TJson>(string Input)
        {
            if (string.IsNullOrWhiteSpace(Input))
                return default;

            try { return JsonConvert.DeserializeObject<TJson>(Input); }
            catch
            {
            }

            return default;
        }
    }
}
