using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Efcore.Notation.Extensions.Notations.Conversions
{
    /// <summary>
    /// Store the property as string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class StoreAsJsonAttribute : ConversionAttribute
    {
        /// <inheritdoc/>
        protected override string Stringify<TProperty>(TProperty Property)
        {
            if (Property is null)
                return string.Empty;

            try { return JsonConvert.SerializeObject(Property, Formatting.None); }
            catch
            {
            }

            return string.Empty;
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
                Property = JsonConvert.DeserializeObject<TProperty>(Input);
                return true;
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
