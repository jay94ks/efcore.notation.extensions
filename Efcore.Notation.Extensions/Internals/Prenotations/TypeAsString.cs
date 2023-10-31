using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection;

namespace Efcore.Notation.Extensions.Internals.Prenotations
{
    /// <summary>
    /// Store <see cref="Type"/> as string.
    /// </summary>
    internal class TypeAsString
    {
        private static readonly HashSet<Assembly> ASSEMBLIES = new();

        /// <summary>
        /// Configure the property builder.
        /// </summary>
        /// <param name="Property"></param>
        public static void Configure(PropertyBuilder Property)
        {
            if (Property is not PropertyBuilder<Type> Type)
                return;

            Type.HasConversion(
                new ValueConverter<Type, string>(X => ToString(X), X => SafeParse(X)),
                new ValueComparer<Type>((A, B) => ToString(A) == ToString(B),
                    X => ToString(X).GetHashCode(), X => SafeParse(ToString(X))));

            Type.HasMaxLength(255);
        }

        /// <summary>
        /// Convert <see cref="Type"/> to string without exception.
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        internal static string ToString(Type Type)
        {
            if (Type is null)
                return string.Empty;

            return Type.FullName;
        }

        /// <summary>
        /// Parse the input string without exception.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        internal static Type SafeParse(string Input)
        {
            if (string.IsNullOrWhiteSpace(Input))
                return null;

            try
            {
                var Type = System.Type.GetType(Input);
                if (Type != null)
                    return Type;
            }

            catch { }

            foreach (var Each in GetAssemblies())
            {
                try
                {
                    var Type = Each.GetType(Input);
                    if (Type != null)
                        return Type;
                }

                catch { }
            }

            return null;
        }

        /// <summary>
        /// Get assemblies. this make cache that `<see cref="ScanAssemblies"/>` method returned.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Assembly> GetAssemblies()
        {
            lock(ASSEMBLIES)
            {
                if (ASSEMBLIES.Count > 0)
                    return ASSEMBLIES;
            }

            var Assemblies = ScanAssemblies().ToArray();
            lock (ASSEMBLIES)
            {
                foreach (var Each in Assemblies)
                    ASSEMBLIES.Add(Each);

                return ASSEMBLIES;
            }
        }

        /// <summary>
        /// Scan all assemblies recursively.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Assembly> ScanAssemblies()
        {
            var Entry = Assembly.GetEntryAssembly();
            var Executing = Assembly.GetExecutingAssembly();
            var Calling = Assembly.GetExecutingAssembly();

            var Set = new HashSet<Assembly>();

            if (Set.Add(Entry))
                yield return Entry;

            if (Set.Add(Executing))
                yield return Executing;

            if (Set.Add(Calling))
                yield return Calling;

            var Queue = new Queue<Assembly>(Set);
            while(Queue.TryDequeue(out var Each))
            {
                var Refs = Each.GetReferencedAssemblies();
                if (Refs is null || Refs.Length <= 0)
                    continue;

                foreach(var Ref in Refs)
                {
                    Assembly Loaded = null;
                    try
                    {
                        if ((Loaded = Assembly.Load(Ref)) is null)
                            continue;

                        if (Set.Add(Loaded) == false)
                            continue;

                        if (Set.FirstOrDefault(X => X.FullName == Loaded.FullName) != null)
                            continue;
                    }
                    catch { }

                    if (Loaded != null)
                    {
                        Queue.Enqueue(Loaded);
                        yield return Loaded;
                    }
                }
            }
        }
    }
}
