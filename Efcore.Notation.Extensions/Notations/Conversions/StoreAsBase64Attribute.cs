using Microsoft.EntityFrameworkCore;

namespace Efcore.Notation.Extensions.Notations.Conversions
{
    /// <summary>
    /// Store <see cref="byte"/> array as BASE64 string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class StoreAsBase64Attribute : ConversionAttribute
    {
        /// <summary>
        /// Base64 encoding options.
        /// </summary>
        public Base64FormattingOptions Options { get; set; } = Base64FormattingOptions.None;

        /// <inheritdoc/>
        protected override bool CanSupport<TProperty>()
        {
            if (typeof(TProperty) == typeof(ArraySegment<byte>))
                return true;

            return typeof(TProperty).IsAssignableFrom(typeof(byte[]));
        }

        /// <inheritdoc/>
        protected override string Stringify<TProperty>(TProperty Property)
        {
            if (Property is byte[] Array)
            {
                if (Array.Length <= 0)
                    return string.Empty;

                return Convert.ToBase64String(Array, Options);
            }

            if (Property is ArraySegment<byte> Segment)
            {
                if (Segment.Array is null)
                    return string.Empty;

                return Convert.ToBase64String(Segment.Array, Segment.Offset, Segment.Count, Options);
            }

            if (Property is IEnumerable<byte> Enumerable)
            {
                if (Enumerable.Count() <= 0)
                    return string.Empty;

                return Stringify(Enumerable.ToArray());
            }

            return string.Empty;
        }

        /// <inheritdoc/>
        protected override bool TryParse<TProperty>(string Input, out TProperty Property)
        {
            if (CanSupport<TProperty>() == false)
            {
                Property = default;
                return false;
            }

            if (string.IsNullOrWhiteSpace(Input))
            {
                if (typeof(TProperty) == typeof(byte[]))
                    Property = (TProperty)((object)Array.Empty<byte>());

                else if (typeof(TProperty) == typeof(ArraySegment<byte>))
                    Property = (TProperty)((object)new ArraySegment<byte>(Array.Empty<byte>()));

                else if (typeof(TProperty).IsAssignableFrom(typeof(IEnumerable<byte>)))
                    Property = (TProperty)((object)Array.Empty<byte>().AsEnumerable());

                else
                {
                    Property = default;
                    return false;
                }

                return true;
            }

            try
            {
                var Result = Convert.FromBase64String(Input);
                if (typeof(TProperty) == typeof(byte[]))
                    Property = (TProperty)((object)Result);

                else if (typeof(TProperty) == typeof(ArraySegment<byte>))
                    Property = (TProperty)((object)new ArraySegment<byte>(Result));

                else if (typeof(TProperty).IsAssignableFrom(typeof(IEnumerable<byte>)))
                    Property = (TProperty)((object)Result.AsEnumerable());

                else
                {
                    Property = default;
                    return false;
                }

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
