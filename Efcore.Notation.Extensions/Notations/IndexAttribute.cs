namespace Efcore.Notation.Extensions.Notations
{
    /// <summary>
    /// Index attribute.
    /// Marks a property as index.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class IndexAttribute : NotationAttribute
    {
        /// <summary>
        /// Name of index.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Order.
        /// </summary>
        public int Order { get; set; } = int.MaxValue / 2;

        /// <summary>
        /// Collection.
        /// </summary>
        private class Collection : HashSet<(string Name, int Order, string Property)>
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
                    foreach(var Each in Collection.GroupBy(X => X.Name))
                    {
                        var Properties = Each
                            .OrderBy(X => X.Order).Select(X => X.Property)
                            .ToArray();

                        Context.Entity.HasIndex(Properties, Each.Key).IsUnique(false);
                    }
                });
            }

            var Name = this.Name;
            if (string.IsNullOrWhiteSpace(Name))
            {
                var Property = Context.PropertyInfo.Name.ToUpper();
                var Entity = Context.PropertyInfo.DeclaringType.Name.ToUpper();

                Name = $"IX_{Entity}_{Property}";
            }

            Collection.Add((Name, Order, Property: Context.PropertyInfo.Name));
        }
    }
}
