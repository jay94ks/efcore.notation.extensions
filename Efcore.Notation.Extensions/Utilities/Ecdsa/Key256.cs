using Secp256k1Net;
using SimpleBase;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Efcore.Notation.Extensions.Utilities.Ecdsa
{
    /// <summary>
    /// 256-bit Private Key. (SECP256K1)
    /// </summary>
    public readonly struct Key256 : IEquatable<Key256>, IEquatable<string>, IComparable<Key256>, IComparable<string>, IEnumerable<byte>
    {
        private static readonly byte[] ZERO_RAW = new byte[Size];
        private static readonly string ZERO_B58 = Base58.Bitcoin.Encode(ZERO_RAW);

        // --
        private readonly byte[] m_Raw;
        private readonly string m_B58;

        /// <summary>
        /// Size in bytes.
        /// </summary>
        public const int Size = Secp256k1.PRIVKEY_LENGTH;

        /// <summary>
        /// Zero.
        /// </summary>
        public static readonly Key256 Zero = new Key256();

        /// <summary>
        /// Initialize a new <see cref="Key256"/> value.
        /// </summary>
        public Key256()
        {
            m_Raw = ZERO_RAW;
            m_B58 = ZERO_B58;
        }

        /// <summary>
        /// Initialize a new <see cref="Key256"/> value.
        /// </summary>
        /// <param name="Raw"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Key256(byte[] Raw)
        {
            if (Raw is null)
                throw new ArgumentNullException(nameof(Raw));

            if (Raw.Length != Size)
                Array.Resize(ref m_Raw, Size);

            m_Raw = Raw;
            m_B58 = Base58.Bitcoin.Encode(Raw);
        }

        /// <summary>
        /// Initialize a new <see cref="Key256"/> value.
        /// </summary>
        /// <param name="B58"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        public Key256(string B58)
        {
            if (string.IsNullOrWhiteSpace(B58))
                throw new ArgumentNullException(nameof(B58));

            var Raw = Base58.Bitcoin.Decode(B58);
            if (Raw.Length != Size)
                Array.Resize(ref m_Raw, Size);

            m_Raw = Raw;
            m_B58 = B58;
        }

        /// <summary>
        /// Make a new <see cref="Key256"/>.
        /// </summary>
        /// <returns></returns>
        public static Key256 New()
        {
            using var Rng = RandomNumberGenerator.Create();
            using var Secp = new Secp256k1();
            Span<byte> Key = stackalloc byte[Size];

            while (true)
            {
                Rng.GetNonZeroBytes(Key);

                if (Secp.SecretKeyVerify(Key) == false)
                    continue;

                break;
            }

            return new Key256(Key.ToArray());
        }

        /// <summary>
        /// Make <see cref="Pub256"/>.
        /// </summary>
        /// <returns></returns>
        public Pub256 ToPub256()
        {
            if (B58 == ZERO_B58)
                return default;

            try
            {
                using var Secp = new Secp256k1();
                Span<byte> Pub = stackalloc byte[Secp256k1.PUBKEY_LENGTH];
                if (Secp.PublicKeyCreate(Pub, Raw) == false)
                    return default;

                Span<byte> Enc = stackalloc byte[Pub256.Size];
                if (Secp.PublicKeySerialize(Enc, Pub, Flags.SECP256K1_EC_COMPRESSED) == false)
                    return default;

                return new Pub256(Enc.ToArray());
            }

            catch { }
            return default;
        }

        /// <summary>
        /// Sign the digest using this key.
        /// </summary>
        /// <param name="Digest"></param>
        /// <returns></returns>
        public Sign256 Sign(Hash256 Digest)
        {
            if (B58 == ZERO_B58)
                return default;

            try
            {
                using var Secp = new Secp256k1();
                Span<byte> Sig = stackalloc byte[Secp256k1.SIGNATURE_LENGTH];

                if (Secp.Sign(Sig, Digest.Raw, Raw) == false)
                    return default;

                return new Sign256(Sig.ToArray());
            }

            catch { }
            return default;
        }

        /// <summary>
        /// Raw bytes.
        /// </summary>
        internal byte[] Raw => m_Raw ?? ZERO_RAW;

        /// <summary>
        /// Hex string.
        /// </summary>
        internal string B58 => m_B58 ?? ZERO_B58;

        /// <summary>
        /// Array accessor.
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public byte this[int Index] => Raw[Index];

        /// <inheritdoc/>
        public bool Equals(Key256 Other) => B58.Equals(Other.B58);

        /// <inheritdoc/>
        public bool Equals(string Other) => B58.Equals(Other ?? string.Empty);

        /// <inheritdoc/>
        public override string ToString() => B58;

        /// <inheritdoc/>
        public override int GetHashCode() => B58.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object Input)
        {
            if (Input is null)
                return false;

            switch (Input)
            {
                case Key256 Hash: return Equals(Hash);
                case string Hexs: return Equals(Hexs);
                default:
                    break;
            }

            return false;
        }

        /// <inheritdoc/>
        public IEnumerator<byte> GetEnumerator() => Raw.AsEnumerable().GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => Raw.GetEnumerator();

        /// <inheritdoc/>
        public int CompareTo(Key256 Other) => B58.CompareTo(Other.B58);

        /// <inheritdoc/>
        public int CompareTo(string Other) => B58.CompareTo(Other ?? string.Empty);

        /// <summary>
        /// Copy bytes to the buffer.
        /// </summary>
        /// <param name="Buffer"></param>
        public void CopyTo(Span<byte> Buffer) => Raw.CopyTo(Buffer);

        /// <summary>
        /// L == R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator ==(Key256 L, Key256 R) => L.Equals(R) == true;

        /// <summary>
        /// L == R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator ==(Key256 L, string R) => L.Equals(R) == true;

        /// <summary>
        /// L != R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator !=(Key256 L, Key256 R) => L.Equals(R) == false;

        /// <summary>
        /// L != R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator !=(Key256 L, string R) => L.Equals(R) == false;

        /// <summary>
        /// L <= R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator <=(Key256 L, Key256 R) => L.CompareTo(R) <= 0;

        /// <summary>
        /// L >= R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator >=(Key256 L, Key256 R) => L.CompareTo(R) >= 0;

        /// <summary>
        /// L < R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator <(Key256 L, Key256 R) => L.CompareTo(R) < 0;

        /// <summary>
        /// L > R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator >(Key256 L, Key256 R) => L.CompareTo(R) > 0;

        /// <summary>
        /// L <= R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator <=(Key256 L, string R) => L.CompareTo(R) <= 0;

        /// <summary>
        /// L >= R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator >=(Key256 L, string R) => L.CompareTo(R) >= 0;

        /// <summary>
        /// L < R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator <(Key256 L, string R) => L.CompareTo(R) < 0;

        /// <summary>
        /// L > R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator >(Key256 L, string R) => L.CompareTo(R) > 0;

    }
}
