using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace Efcore.Notation.Extensions.Notations.Conversions
{
    /// <summary>
    /// Store the property as string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class StoreAsBsonAttribute : ConversionAttribute
    {
        /// <inheritdoc/>
        protected override string Stringify<TProperty>(TProperty Property)
        {
            if (Property is null)
                return string.Empty;

            using var Stream = new MemoryStream();
            using (var Writer = new BsonDataWriter(Stream))
            {
                try
                {
                    JsonSerializer
                        .CreateDefault()
                        .Serialize(Writer, Property);
                }
                catch
                {
                    return string.Empty;
                }
            }

            return Convert.ToBase64String(Stream.ToArray());
        }

        /// <inheritdoc/>
        protected override bool TryParse<TProperty>(string Input, out TProperty Property)
        {
            if (string.IsNullOrWhiteSpace(Input))
            {
                Property = default;
                return false;
            }
            try
            {
                var Data = Convert.FromBase64String(Input);
                if (Data is null || Data.Length <= 0)
                {
                    Property = default;
                    return false;
                }

                using var Stream = new MemoryStream(Data, false);
                using (var Reader = new BsonDataReader(Stream))
                {
                    Property = JsonSerializer
                        .CreateDefault()
                        .Deserialize<TProperty>(Reader);

                    return true;
                }
            }

            catch { }

            Property = default;
            return false;
        }

        /// <inheritdoc/>
        protected override void OnConfigureProperty<TEntity, TProperty>(NotationContext<TEntity, TProperty> Context)
        {
            base.OnConfigureProperty(Context);
            Context.Property.HasColumnType("LONGTEXT");
        }
    }
}
