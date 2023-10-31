using System.Security.Cryptography;

namespace Efcore.Notation.Extensions.Utilities.Internals.Passwords
{
    /// <summary>
    /// PKBDF2.
    /// </summary>
    internal class Pbkdf2Algorithm : Password.Algorithm
    {
        private readonly byte[] m_Salt;
        private readonly int m_SizeInBytes;
        private readonly int m_Iteration;
        private readonly HashAlgorithmName m_Algorithm;

        /// <summary>
        /// Initialize a new <see cref="Pbkdf2Algorithm"/> instance.
        /// </summary>
        /// <param name="Salt"></param>
        /// <param name="SizeInBytes"></param>
        public Pbkdf2Algorithm(byte[] Salt, int SizeInBytes = 32, int Iteration = 210000)
        {
            if (Salt is null)
                throw new ArgumentNullException(nameof(Salt));

            if (SizeInBytes <= 15)
                throw new ArgumentException($"Size in bytes should be greater than 15 bytes.");

            m_Salt = Salt;

            if (m_Salt.Length != SizeInBytes / 2)
                Array.Resize(ref m_Salt, SizeInBytes / 2);

            m_Iteration = Iteration;
            m_Algorithm = HashAlgorithmName.SHA512;
            m_SizeInBytes = SizeInBytes;

            Aliases = new string[]
            {
                "pbkdf2-" + (SizeInBytes * 8),
                "pbkdf2-sha512-" + (SizeInBytes * 8)
            };
        }

        /// <summary>
        /// Initialize a new <see cref="Pbkdf2Algorithm"/> instance.
        /// </summary>
        /// <param name="Salt"></param>
        /// <param name="SizeInBytes"></param>
        public Pbkdf2Algorithm(byte[] Salt, HashAlgorithmName Algorithm, int SizeInBytes = 32, int Iteration = 210000)
        {
            if (Salt is null)
                throw new ArgumentNullException(nameof(Salt));

            if (SizeInBytes <= 15)
                throw new ArgumentException($"Size in bytes should be greater than 15 bytes.");

            m_Salt = Salt;

            if (m_Salt.Length != SizeInBytes / 2)
                Array.Resize(ref m_Salt, SizeInBytes / 2);

            m_Iteration = Iteration;
            m_Algorithm = Algorithm;
            m_SizeInBytes = SizeInBytes;

            Aliases = new string[]
            {
                "pbkdf2-" + Algorithm.Name + "-" + (SizeInBytes * 8)
            };
        }

        /// <inheritdoc/>
        public override int SizeInBytes => m_SizeInBytes;

        /// <inheritdoc/>
        public override string Name => "pbkdf2";

        /// <inheritdoc/>
        public override IEnumerable<string> Aliases { get; } 

        /// <inheritdoc/>
        public override byte[] Compute(string Input)
        {
            using var Pbkdf2 = new Rfc2898DeriveBytes(
                Input, m_Salt, m_Iteration, m_Algorithm);

            return Pbkdf2.GetBytes(m_SizeInBytes);
        }
    }
}
