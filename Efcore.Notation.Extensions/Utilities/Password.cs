using Efcore.Notation.Extensions.Utilities.Internals.Passwords;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Efcore.Notation.Extensions.Utilities
{
    /// <summary>
    /// Password.
    /// </summary>
    public readonly partial struct Password : IEquatable<Password>, IEquatable<string>
    {
        private static readonly List<Algorithm> ALGORITHMS = new()
        {
            { new Sha256Algorithm() },
            { new Sha384Algorithm() },
            { new Sha512Algorithm() },
            { new Md5Algorithm() },
            { new Pbkdf2Algorithm(Encoding.UTF8.GetBytes("EFCORE_NEUTILS_PASSWORD_SALT"), 32) },
        };

        // --
        private readonly string m_Algorithm;
        private readonly string m_HashValue;

        /// <summary>
        /// Configure the password property.
        /// </summary>
        /// <param name="Property"></param>
        internal static void Configure(PropertyBuilder Property)
        {
            if (Property is not PropertyBuilder<Password> Password)
                return;

            Password.HasConversion(
                new ValueConverter<Password, string>(X => X.ToString(), X => ParseNoExcept(X)),
                new ValueComparer<Password>((A, B) => A == B, X => X.GetHashCode(), X => X));

            Password.HasMaxLength(255);
        }

        /// <summary>
        /// Empty password.
        /// </summary>
        public static readonly Password Empty = new();

        /// <summary>
        /// Initialize a new <see cref="Password"/> instance.
        /// </summary>
        public Password()
        {
            m_Algorithm = string.Empty;
            m_HashValue = string.Empty;
        }

        /// <summary>
        /// Initialize a new <see cref="Password"/> instance.
        /// </summary>
        /// <param name="HashValue"></param>
        /// <param name="Algorithm"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Password(byte[] HashValue, string Algorithm)
        {
            if (HashValue is null || HashValue.Length <= 0)
            {
                m_Algorithm = string.Empty;
                m_HashValue = string.Empty;
                return;
            }

            if (string.IsNullOrWhiteSpace(Algorithm))
                throw new ArgumentNullException(nameof(Algorithm));

            m_Algorithm = Algorithm;
            m_HashValue = Convert.ToHexString(HashValue).ToLower();
        }

        /// <summary>
        /// Parse password.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static Password Parse(string Input)
        {
            if (string.IsNullOrWhiteSpace(Input))
                return default;

            var Temp = Input.Split(':', 2, StringSplitOptions.None);
            if (Temp.Length != 2)
                throw new FormatException("password string format must be `algorithm:hash-in-hex`.");

            var Data = Convert.FromHexString(Temp.Last());
            return new Password(Data, Temp.First());
        }

        /// <summary>
        /// Parse password.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        private static Password ParseNoExcept(string Input)
        {
            if (string.IsNullOrWhiteSpace(Input))
                return Empty;

            var Temp = Input.Split(':', 2, StringSplitOptions.None);
            if (Temp.Length != 2)
                return Empty;

            try
            {
                var Data = Convert.FromHexString(Temp.Last());
                return new Password(Data, Temp.First());
            }

            catch { }
            return Empty;
        }

        /// <summary>
        /// Try to parse password.
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static bool TryParse(string Input, out Password Password)
        {
            if (string.IsNullOrWhiteSpace(Input))
            {
                Password = Empty;
                return false;
            }

            var Temp = Input.Split(':', 2, StringSplitOptions.None);
            if (Temp.Length != 2)
            {
                Password = Empty;
                return false;
            }

            try
            {
                var Data = Convert.FromHexString(Temp.Last());
                Password = new Password(Data, Temp.First());
                return true;
            }

            catch { }

            Password = Empty;
            return false;
        }

        /// <summary>
        /// Make a password using algorithm.
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Algorithm"></param>
        /// <returns></returns>
        public static Password Make(string Text, Algorithm Algorithm = null)
        {
            if (Algorithm is null)
                Algorithm = Algorithm.Default;

            var Data = Algorithm.Compute(Text ?? string.Empty);
            return new Password(Data, Algorithm.Name);
        }

        /// <summary>
        /// Make a password using algorithm.
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="AlgorithmName"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">algorithm is not supported.</exception>
        public static Password Make(string Text, string AlgorithmName)
        {
            if (string.IsNullOrWhiteSpace(AlgorithmName))
                return Make(Text, Algorithm.Default);

            var Instance = Algorithm.Get(AlgorithmName);
            if (Instance is null)
                throw new NotSupportedException($"Algorithm is not supported: {AlgorithmName}.");

            return Make(Text, Instance);
        }

        /// <inheritdoc/>
        public bool Equals(Password other)
        {
            if (m_Algorithm != other.m_Algorithm)
                return false;

            if (m_HashValue != other.m_HashValue)
                return false;

            return true;
        }

        /// <inheritdoc/>
        public bool Equals(string Other)
        {
            if (string.IsNullOrWhiteSpace(m_Algorithm))
                return string.IsNullOrWhiteSpace(Other);

            return Equals(Make(Other, m_Algorithm));
        }

        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object Input)
        {
            switch(Input)
            {
                case Password Password: return Equals(Password);
                case string String: return Equals(String);
                default: break;
            }

            return false;
        }

        /// <summary>
        /// Compare two passwords.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Password a, Password b) => a.Equals(b) == true;

        /// <summary>
        /// Compare two passwords.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Password a, string b) => a.Equals(b) == true;

        /// <summary>
        /// Compare two passwords.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Password a, Password b) => a.Equals(b) == false;

        /// <summary>
        /// Compare two passwords.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Password a, string b) => a.Equals(b) == false;

        /// <inheritdoc/>
        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(m_Algorithm))
                return string.Empty;

            return $"{m_Algorithm}:{m_HashValue}";
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(m_Algorithm, m_HashValue);
        }
    }
}
