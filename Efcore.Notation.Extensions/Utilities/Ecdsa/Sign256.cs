using Secp256k1Net;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Efcore.Notation.Extensions.Utilities.Ecdsa
{
    /// <summary>
    /// 256-bit Signature. (SECP256K1)
    /// </summary>
    public readonly struct Sign256 : IEquatable<Sign256>, IEquatable<string>, IComparable<Sign256>, IComparable<string>, IEnumerable<byte>
    {
        private static readonly byte[] ZERO_RAW = new byte[Size];
        private static readonly string ZERO_HEX = Convert.ToHexString(ZERO_RAW);

        // --
        private readonly byte[] m_Raw;
        private readonly string m_Hex;

        /// <summary>
        /// Size in bytes.
        /// </summary>
        public const int Size = Secp256k1.SIGNATURE_LENGTH;

        /// <summary>
        /// Zero.
        /// </summary>
        public static readonly Sign256 Zero = new Sign256();

        /// <summary>
        /// Initialize a new <see cref="Sign256"/> value.
        /// </summary>
        public Sign256()
        {
            m_Raw = ZERO_RAW;
            m_Hex = ZERO_HEX;
        }

        /// <summary>
        /// Initialize a new <see cref="Sign256"/> value.
        /// </summary>
        /// <param name="Raw"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Sign256(byte[] Raw)
        {
            if (Raw is null)
                throw new ArgumentNullException(nameof(Raw));

            if (Raw.Length != Size)
                Array.Resize(ref m_Raw, Size);

            m_Raw = Raw;
            m_Hex = Convert.ToHexString(Raw).ToLower();
        }

        /// <summary>
        /// Initialize a new <see cref="Sign256"/> value.
        /// </summary>
        /// <param name="Hex"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        public Sign256(string Hex)
        {
            if (string.IsNullOrWhiteSpace(Hex))
                throw new ArgumentNullException(nameof(Hex));

            var Raw = Convert.FromHexString(Hex);
            if (Raw.Length != Size)
                Array.Resize(ref m_Raw, Size);

            m_Raw = Raw;
            m_Hex = Hex.ToLower();
        }

        /// <summary>
        /// Verify the signature using specified public key.
        /// </summary>
        /// <param name="Pub"></param>
        /// <param name="Digest"></param>
        /// <returns></returns>
        public bool Verify(Pub256 Pub, Hash256 Digest) => Pub.Verify(this, Digest);

        /// <summary>
        /// Raw bytes.
        /// </summary>
        internal byte[] Raw => m_Raw ?? ZERO_RAW;

        /// <summary>
        /// Hex string.
        /// </summary>
        internal string Hex => m_Hex ?? ZERO_HEX;

        /// <summary>
        /// Array accessor.
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public byte this[int Index] => Raw[Index];

        /// <inheritdoc/>
        public bool Equals(Sign256 Other) => Hex.Equals(Other.Hex, StringComparison.OrdinalIgnoreCase);

        /// <inheritdoc/>
        public bool Equals(string Other) => Hex.Equals(Other ?? string.Empty, StringComparison.OrdinalIgnoreCase);

        /// <inheritdoc/>
        public override string ToString() => Hex;

        /// <inheritdoc/>
        public override int GetHashCode() => Hex.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object Input)
        {
            if (Input is null)
                return false;

            switch (Input)
            {
                case Sign256 Hash: return Equals(Hash);
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
        public int CompareTo(Sign256 Other) => Hex.CompareTo(Other.Hex);

        /// <inheritdoc/>
        public int CompareTo(string Other) => Hex.CompareTo((Other ?? string.Empty).ToLower());

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
        public static bool operator ==(Sign256 L, Sign256 R) => L.Equals(R) == true;

        /// <summary>
        /// L == R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator ==(Sign256 L, string R) => L.Equals(R) == true;

        /// <summary>
        /// L != R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator !=(Sign256 L, Sign256 R) => L.Equals(R) == false;

        /// <summary>
        /// L != R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator !=(Sign256 L, string R) => L.Equals(R) == false;

        /// <summary>
        /// L <= R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator <=(Sign256 L, Sign256 R) => L.CompareTo(R) <= 0;

        /// <summary>
        /// L >= R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator >=(Sign256 L, Sign256 R) => L.CompareTo(R) >= 0;

        /// <summary>
        /// L < R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator <(Sign256 L, Sign256 R) => L.CompareTo(R) < 0;

        /// <summary>
        /// L > R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator >(Sign256 L, Sign256 R) => L.CompareTo(R) > 0;

        /// <summary>
        /// L <= R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator <=(Sign256 L, string R) => L.CompareTo(R) <= 0;

        /// <summary>
        /// L >= R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator >=(Sign256 L, string R) => L.CompareTo(R) >= 0;

        /// <summary>
        /// L < R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator <(Sign256 L, string R) => L.CompareTo(R) < 0;

        /// <summary>
        /// L > R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator >(Sign256 L, string R) => L.CompareTo(R) > 0;
    }
}
