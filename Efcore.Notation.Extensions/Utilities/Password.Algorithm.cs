namespace Efcore.Notation.Extensions.Utilities
{
    public partial struct Password
    {
        /// <summary>
        /// Base class for algorithms.
        /// </summary>
        public abstract class Algorithm
        {
            private static readonly AsyncLocal<Algorithm> DEFAULTS = new();
            private static Algorithm[] CACHED_ALL = null;

            /// <summary>
            /// Get or set the default algorithm instance.
            /// </summary>
            public static Algorithm Default
            {
                get => DEFAULTS.Value ?? ALGORITHMS.FirstOrDefault();
                set
                {
                    if ((DEFAULTS.Value = value) is null)
                        return;

                    lock (ALGORITHMS)
                    {
                        var Index = ALGORITHMS.FindIndex(X => X == value);
                        if (Index <= 0)
                            return;

                        var Temp = ALGORITHMS[0];
                        ALGORITHMS[0] = ALGORITHMS[Index];
                        ALGORITHMS[Index] = Temp;
                    }
                }
            }

            /// <summary>
            /// Get all algorithms supported.
            /// </summary>
            public static Algorithm[] All
            {
                get
                {
                    lock (ALGORITHMS)
                    {
                        if (CACHED_ALL is null)
                            CACHED_ALL = ALGORITHMS.ToArray();

                        return CACHED_ALL;
                    }
                }
            }

            /// <summary>
            /// Register an algorithm to registry.
            /// </summary>
            /// <param name="Algorithm"></param>
            /// <returns></returns>
            public static bool Register(Algorithm Algorithm)
            {
                if (Algorithm is null)
                    throw new ArgumentNullException(nameof(Algorithm));

                lock (ALGORITHMS)
                {
                    if (Supports(Algorithm.Name))
                        return false;

                    ALGORITHMS.Add(Algorithm);
                    CACHED_ALL = null;
                    return true;
                }
            }

            /// <summary>
            /// Unregister an algorithm from registry.
            /// </summary>
            /// <param name="Algorithm"></param>
            /// <returns></returns>
            public static bool Unregister(Algorithm Algorithm)
            {
                if (Algorithm is null)
                    throw new ArgumentNullException(nameof(Algorithm));

                if (Algorithm.GetType().Assembly == typeof(Password).Assembly)
                    return false;

                lock (ALGORITHMS)
                {
                    var Index = ALGORITHMS.IndexOf(Algorithm);
                    if (Index < 0)
                        return false;

                    ALGORITHMS.RemoveAt(Index);
                    CACHED_ALL = null;
                    return true;
                }
            }

            /// <summary>
            /// Test whether the algorithm is supported or not.
            /// </summary>
            /// <param name="Name"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException"></exception>
            public static bool Supports(string Name)
            {
                if (string.IsNullOrWhiteSpace(Name))
                    throw new ArgumentNullException(nameof(Name));

                return Get(Name) != null;
            }

            /// <summary>
            /// Get the algorithm by its name.
            /// If the algorithm is not supported, this returns null.
            /// </summary>
            /// <param name="Name"></param>
            /// <returns></returns>
            public static Algorithm Get(string Name)
            {
                if (string.IsNullOrWhiteSpace(Name))
                    throw new ArgumentNullException(nameof(Name));

                Algorithm Algorithm;
                lock (ALGORITHMS)
                {
                    Algorithm = ALGORITHMS.Find(X =>
                    {
                        if (X.Name.Equals(Name, StringComparison.OrdinalIgnoreCase))
                            return true;

                        if (X.Aliases.Select(X => X.ToLower()).Contains(Name))
                            return true;

                        return false;
                    });
                }

                return Algorithm;
            }

            /// <summary>
            /// Get the algorithm by its name. This finds algorithm with fallbacks.
            /// If the algorithm is not supported, this returns null.
            /// </summary>
            /// <param name="Names"></param>
            /// <returns></returns>
            public static Algorithm Get(params string[] Names)
            {
                if (Names is null)
                    throw new ArgumentNullException(nameof(Names));

                foreach(var Each in Names)
                {
                    var Algorithm = Get(Each);
                    if (Algorithm != null)
                        return Algorithm;
                }

                return null;
            }

            /// <summary>
            /// Size in bytes.
            /// </summary>
            public abstract int SizeInBytes { get; }

            /// <summary>
            /// Name.
            /// </summary>
            public abstract string Name { get; }

            /// <summary>
            /// Aliases.
            /// </summary>
            public abstract IEnumerable<string> Aliases { get; }

            /// <summary>
            /// Compute hash of the input string.
            /// </summary>
            /// <param name="Input"></param>
            /// <returns></returns>
            public abstract byte[] Compute(string Input);
        }


    }
}
