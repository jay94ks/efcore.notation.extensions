using System.Security.Cryptography;
using System.Text;

namespace Efcore.Notation.Extensions.Utilities.Internals.Passwords
{
    /// <summary>
    /// <see cref="SHA384"/> wrapper for <see cref="Password.Algorithm"/>s.
    /// </summary>
    internal class Sha384Algorithm : Password.Algorithm
    {
        /// <inheritdoc/>
        public override int SizeInBytes => 384 / 8;

        /// <inheritdoc/>
        public override string Name => "sha384";

        /// <inheritdoc/>
        public override IEnumerable<string> Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public override byte[] Compute(string Input)
        {
            using var Sha = SHA384.Create();
            return Sha.ComputeHash(Encoding.UTF8.GetBytes(Input ?? string.Empty));
        }
    }
}
