using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Efcore.Notation.Extensions
{
    /// <summary>
    /// Type Sets to apply notations.
    /// </summary>
    public class ExtendedDbEntitySet : HashSet<Type>
    {
        /// <summary>
        /// Add a type to the set.
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public new ExtendedDbEntitySet Add(Type Type)
        {
            base.Add(Type);
            return this;
        }

        /// <summary>
        /// Add types from assembly.
        /// This adds all types which have <see cref="TableAttribute"/> if no filter specified.
        /// </summary>
        /// <param name="Assembly"></param>
        /// <param name="Filter"></param>
        /// <returns></returns>
        public ExtendedDbEntitySet AddFrom(Assembly Assembly, Func<Type, bool> Filter = null)
        {
            var Types = Assembly.GetTypes()
                .Where(X => X.GetCustomAttribute<TableAttribute>() != null)
                ;

            if (Filter != null)
                Types = Types.Where(Filter);

            foreach (var Each in Types)
                base.Add(Each);

            return this;
        }
    }
}
