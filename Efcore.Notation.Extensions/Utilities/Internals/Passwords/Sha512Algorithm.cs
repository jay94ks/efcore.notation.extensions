using System.Security.Cryptography;
using System.Text;

namespace Efcore.Notation.Extensions.Utilities.Internals.Passwords
{
    /// <summary>
    /// <see cref="SHA512"/> wrapper for <see cref="Password.Algorithm"/>s.
    /// </summary>
    internal class Sha512Algorithm : Password.Algorithm
    {
        /// <inheritdoc/>
        public override int SizeInBytes => 512 / 8;

        /// <inheritdoc/>
        public override string Name => "sha512";

        /// <inheritdoc/>
        public override IEnumerable<string> Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public override byte[] Compute(string Input)
        {
            using var Sha = SHA512.Create();
            return Sha.ComputeHash(Encoding.UTF8.GetBytes(Input ?? string.Empty));
        }
    }
}
