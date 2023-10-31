using Efcore.Notation.Extensions.Internals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;

namespace Efcore.Notation.Extensions
{
    /// <summary>
    /// Database property context.
    /// </summary>
    public class NotationContext<TEntity, TProperty> where TEntity : class
    {
        /// <summary>
        /// Initialize a new <see cref="NotationContext{TEntity, TProperty}"/> instance.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="Property"></param>
        internal NotationContext(
            ModelContext<TEntity> Context,
            PropertyBuilder<TProperty> Property)
        {
            Items = Context.Items;
            Models = Context.Models;
            Entity = Context.Entity;
            PropertyInfo = Context.PropertyInfo;
            LazyWorks = Context.LazyWorks;
            this.Property = Property;
        }

        /// <summary>
        /// Items to share between properties.
        /// </summary>
        public Dictionary<object, object> Items { get; }

        /// <summary>
        /// Model builder.
        /// </summary>
        public ModelBuilder Models { get; }

        /// <summary>
        /// Entity type builder.
        /// </summary>
        public EntityTypeBuilder<TEntity> Entity { get; }

        /// <summary>
        /// Property Info.
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Property builder.
        /// </summary>
        public PropertyBuilder<TProperty> Property { get; }

        /// <summary>
        /// Lazy works that invoked after configuring properties.
        /// </summary>
        public Queue<Action> LazyWorks { get; }
    }
}
