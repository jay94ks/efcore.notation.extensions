using System.Security.Cryptography;
using System.Text;

namespace Efcore.Notation.Extensions.Utilities.Internals.Passwords
{
    /// <summary>
    /// <see cref="MD5"/> wrapper for <see cref="Password.Algorithm"/>s.
    /// </summary>
    internal class Md5Algorithm : Password.Algorithm
    {
        /// <inheritdoc/>
        public override int SizeInBytes => 512 / 8;

        /// <inheritdoc/>
        public override string Name => "md5";

        /// <inheritdoc/>
        public override IEnumerable<string> Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public override byte[] Compute(string Input)
        {
            using var Md = MD5.Create();
            return Md.ComputeHash(Encoding.UTF8.GetBytes(Input ?? string.Empty));
        }
    }
}
