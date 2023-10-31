using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;

namespace Efcore.Notation.Extensions.Internals
{
    /// <summary>
    /// Entity builder context.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal class ModelContext<TEntity> where TEntity : class
    {
        /// <summary>
        /// Items to share between properties.
        /// </summary>
        public Dictionary<object, object> Items { get; set; } = new();

        /// <summary>
        /// Model builder.
        /// </summary>
        public ModelBuilder Models { get; set; }

        /// <summary>
        /// Entity type builder.
        /// </summary>
        public EntityTypeBuilder<TEntity> Entity { get; set; }

        /// <summary>
        /// Property Info.
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// Lazy works that invoked after configuring properties.
        /// </summary>
        public Queue<Action> LazyWorks { get; set; } = new();
    }
}
