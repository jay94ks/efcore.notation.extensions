namespace Efcore.Notation.Extensions
{
    /// <summary>
    /// Base class for database property attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public abstract class NotationAttribute : Attribute
    {
        /// <summary>
        /// Called to configure <see cref="NotationContext{TEntity, TProperty}"/> instance.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Context"></param>
        protected abstract void OnConfigure<TEntity, TProperty>(NotationContext<TEntity, TProperty> Context) where TEntity : class;

        /// <summary>
        /// Proxy call to <see cref="OnConfigure{TEntity, TProperty}(NotationContext{TEntity, TProperty})"/>.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Context"></param>
        internal void Configure<TEntity, TProperty>(NotationContext<TEntity, TProperty> Context) where TEntity : class => OnConfigure(Context);
    }
}
