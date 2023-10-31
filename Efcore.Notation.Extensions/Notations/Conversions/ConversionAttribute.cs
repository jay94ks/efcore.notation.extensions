using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Efcore.Notation.Extensions.Notations.Conversions
{
    /// <summary>
    /// Base class for database type conversion attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class ConversionAttribute : NotationAttribute
    {
        /// <inheritdoc/>
        protected sealed override void OnConfigure<TEntity, TProperty>(NotationContext<TEntity, TProperty> Context)
        {
            if (CanSupport<TProperty>() == false)
                return;

            Context.Property.HasConversion(
                new ValueConverter<TProperty, string>(X => Stringify(X), X => Parse<TProperty>(X)),
                new ValueComparer<TProperty>((A, B) => Stringify(A) == Stringify(B),
                    X => Stringify(X).GetHashCode(), X => Parse<TProperty>(Stringify(X))));

            OnConfigureProperty(Context);
        }

        /// <summary>
        /// Called when extra configuration required.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Context"></param>
        protected virtual void OnConfigureProperty<TEntity, TProperty>(NotationContext<TEntity, TProperty> Context) where TEntity : class { }

        /// <summary>
        /// Test whether <typeparamref name="TProperty"/> can be supported or not.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        protected virtual bool CanSupport<TProperty>() => true;

        /// <summary>
        /// Parse to <typeparamref name="TProperty"/>.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Input"></param>
        /// <returns></returns>
        private TProperty Parse<TProperty>(string Input)
        {
            if (string.IsNullOrWhiteSpace(Input))
                return default;

            if (TryParse<TProperty>(Input, out var Result) == false)
                return default;

            return Result;
        }

        /// <summary>
        /// Convert to <see cref="string"/>.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Property"></param>
        /// <returns></returns>
        protected abstract string Stringify<TProperty>(TProperty Property);

        /// <summary>
        /// Try to parse to <typeparamref name="TProperty"/>.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Input"></param>
        /// <param name="Property"></param>
        /// <returns></returns>
        protected abstract bool TryParse<TProperty>(string Input, out TProperty Property);
    }
}
