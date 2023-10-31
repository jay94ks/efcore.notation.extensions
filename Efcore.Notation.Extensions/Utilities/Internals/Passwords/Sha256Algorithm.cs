using System.Security.Cryptography;
using System.Text;

namespace Efcore.Notation.Extensions.Utilities.Internals.Passwords
{
    /// <summary>
    /// <see cref="SHA256"/> wrapper for <see cref="Password.Algorithm"/>s.
    /// </summary>
    internal class Sha256Algorithm : Password.Algorithm
    {
        /// <inheritdoc/>
        public override int SizeInBytes => 32;

        /// <inheritdoc/>
        public override string Name => "sha256";

        /// <inheritdoc/>
        public override IEnumerable<string> Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public override byte[] Compute(string Input)
        {
            using var Sha = SHA256.Create();
            return Sha.ComputeHash(Encoding.UTF8.GetBytes(Input ?? string.Empty));
        }
    }
}
