using Microsoft.EntityFrameworkCore;

namespace Efcore.Notation.Extensions.Notations
{
    /// <summary>
    /// Multi-Key attribute.
    /// Marks a property as that has multiple primary keys.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class MultiKeyAttribute : NotationAttribute
    {
        /// <summary>
        /// Order.
        /// </summary>
        public int Order { get; set; } = int.MaxValue / 2;

        /// <summary>
        /// Collection.
        /// </summary>
        private class Collection : HashSet<(int Order, string Property)>
        {

        }

        /// <inheritdoc/>
        protected override void OnConfigure<TEntity, TProperty>(NotationContext<TEntity, TProperty> Context)
        {
            Context.Items.TryGetValue(typeof(Collection), out var Temp);

            if (Temp is not Collection Collection)
            {
                Context.Items[typeof(Collection)] = Collection = new Collection();
                Context.LazyWorks.Enqueue(() =>
                {
                    var Entity = Context.PropertyInfo.DeclaringType.Name.ToUpper();
                    var Properties = Collection
                        .OrderBy(X => X.Order).Select(X => X.Property)
                        .ToArray();

                    Context.Entity.HasKey(Properties).HasName($"PK_{Entity}");
                });
            }

            Collection.Add((Order, Property: Context.PropertyInfo.Name));
        }
    }
}
