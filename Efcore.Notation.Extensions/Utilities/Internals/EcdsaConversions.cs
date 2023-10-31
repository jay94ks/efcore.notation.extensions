using Efcore.Notation.Extensions.Utilities.Ecdsa;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Efcore.Notation.Extensions.Utilities.Internals
{
    /// <summary>
    /// Ecdsa conversions.
    /// </summary>
    internal class EcdsaConversions
    {

        /// <summary>
        /// Configure the property builder.
        /// </summary>
        /// <param name="Property"></param>
        internal static void Configure(PropertyBuilder Property)
        {
            switch (Property)
            {
                case PropertyBuilder<Hash256> Hash256:
                    Hash256.HasConversion(
                        new ValueConverter<Hash256, string>(X => X.ToString(), X => ParseHash256(X)),
                        new ValueComparer<Hash256>((A, B) => A == B, X => X.GetHashCode(), X => X));

                    Hash256.HasMaxLength(Ecdsa.Hash256.Size * 2 + 1);
                    break;

                case PropertyBuilder<Pub256> Pub256:
                    Pub256.HasConversion(
                        new ValueConverter<Pub256, string>(X => X.ToString(), X => ParsePub256(X)),
                        new ValueComparer<Pub256>((A, B) => A == B, X => X.GetHashCode(), X => X));

                    Pub256.HasMaxLength(64);
                    break;

                case PropertyBuilder<Key256> Key256:
                    Key256.HasConversion(
                        new ValueConverter<Key256, string>(X => X.ToString(), X => ParseKey256(X)),
                        new ValueComparer<Key256>((A, B) => A == B, X => X.GetHashCode(), X => X));

                    Key256.HasMaxLength(64);
                    break;

                case PropertyBuilder<Sign256> Sign256:
                    Sign256.HasConversion(
                        new ValueConverter<Sign256, string>(X => X.ToString(), X => ParseSign256(X)),
                        new ValueComparer<Sign256>((A, B) => A == B, X => X.GetHashCode(), X => X));

                    Sign256.HasMaxLength(Ecdsa.Sign256.Size * 2 + 1);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Parse <see cref="Hash256"/>.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        private static Hash256 ParseHash256(string Input)
        {
            try { return new Hash256(Input); }
            catch
            {
            }

            return Hash256.Zero;
        }

        /// <summary>
        /// Parse <see cref="Pub256"/>.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        private static Pub256 ParsePub256(string Input)
        {
            try { return new Pub256(Input); }
            catch
            {
            }

            return Pub256.Zero;
        }

        /// <summary>
        /// Parse <see cref="Key256"/>.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        private static Key256 ParseKey256(string Input)
        {
            try { return new Key256(Input); }
            catch
            {
            }

            return Key256.Zero;
        }

        /// <summary>
        /// Parse <see cref="Sign256"/>.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        private static Sign256 ParseSign256(string Input)
        {
            try { return new Sign256(Input); }
            catch
            {
            }

            return Sign256.Zero;
        }

    }
}
