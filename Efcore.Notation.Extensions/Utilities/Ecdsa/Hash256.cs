﻿using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace Efcore.Notation.Extensions.Utilities.Ecdsa
{
    /// <summary>
    /// 256-bit Hash Value. (SECP256K1)
    /// </summary>
    public readonly struct Hash256 : IEquatable<Hash256>, IEquatable<string>, IComparable<Hash256>, IComparable<string>, IEnumerable<byte>
    {
        private static readonly byte[] ZERO_RAW = new byte[Size];
        private static readonly string ZERO_HEX = Convert.ToHexString(ZERO_RAW);

        // --
        private readonly byte[] m_Raw;
        private readonly string m_Hex;

        /// <summary>
        /// Size in bytes.
        /// </summary>
        public const int Size = 32;

        /// <summary>
        /// Zero.
        /// </summary>
        public static readonly Hash256 Zero = new Hash256();

        /// <summary>
        /// Initialize a new <see cref="Hash256"/> value.
        /// </summary>
        public Hash256()
        {
            m_Raw = ZERO_RAW;
            m_Hex = ZERO_HEX;
        }

        /// <summary>
        /// Initialize a new <see cref="Hash256"/> value.
        /// </summary>
        /// <param name="Raw"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Hash256(byte[] Raw)
        {
            if (Raw is null)
                throw new ArgumentNullException(nameof(Raw));

            if (Raw.Length != Size)
                Array.Resize(ref m_Raw, Size);

            m_Raw = Raw;
            m_Hex = Convert.ToHexString(Raw).ToLower();
        }

        /// <summary>
        /// Initialize a new <see cref="Hash256"/> value.
        /// </summary>
        /// <param name="Hex"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        public Hash256(string Hex)
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
        /// Hash the input stream and returns <see cref="Hash256"/>.
        /// </summary>
        /// <param name="Stream"></param>
        /// <returns></returns>
        public static Hash256 Make(Stream Stream)
        {
            using var SHA = SHA256.Create();
            return new Hash256(SHA.ComputeHash(Stream));
        }

        /// <summary>
        /// Hash from `<paramref name="Action"/>` generated bytes and returns <see cref="Hash256"/>.
        /// </summary>
        /// <param name="Action"></param>
        /// <returns></returns>
        public static Hash256 Make(Action<BinaryWriter> Action)
        {
            using var Stream = new MemoryStream();
            using (var Writer = new BinaryWriter(Stream, Encoding.UTF8, true))
                Action?.Invoke(Writer);

            Stream.Position = 0;
            return Make(Stream);
        }

        /// <summary>
        /// Hash the input data and returns <see cref="Hash256"/>.
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static Hash256 Make(ArraySegment<byte> Data)
        {
            if (Data.Array is null)
                Data = Array.Empty<byte>();

            using var SHA = SHA256.Create();
            return new Hash256(SHA.ComputeHash(
                Data.Array, Data.Offset, Data.Count));
        }

        /// <summary>
        /// Convert <see cref="string"/> to <see cref="Hash256"/> implicitly.
        /// </summary>
        /// <param name="Hex"></param>
        public static implicit operator Hash256(string Hex)
        {
            try { return new Hash256(Hex); }
            catch
            {
            }

            return Zero;
        }

        /// <summary>
        /// Convert <see cref="Name"/> to <see cref="string"/> implicitly.
        /// </summary>
        /// <param name="Hash"></param>
        public static implicit operator string(Hash256 Hash) => Hash.Hex;

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
        public bool Equals(Hash256 Other) => Hex.Equals(Other.Hex, StringComparison.OrdinalIgnoreCase);

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
                case Hash256 Hash: return Equals(Hash);
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
        public int CompareTo(Hash256 Other) => Hex.CompareTo(Other.Hex);

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
        public static bool operator ==(Hash256 L, Hash256 R) => L.Equals(R) == true;

        /// <summary>
        /// L == R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator ==(Hash256 L, string R) => L.Equals(R) == true;

        /// <summary>
        /// L != R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator !=(Hash256 L, Hash256 R) => L.Equals(R) == false;

        /// <summary>
        /// L != R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator !=(Hash256 L, string R) => L.Equals(R) == false;

        /// <summary>
        /// L <= R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator <=(Hash256 L, Hash256 R) => L.CompareTo(R) <= 0;

        /// <summary>
        /// L >= R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator >=(Hash256 L, Hash256 R) => L.CompareTo(R) >= 0;

        /// <summary>
        /// L < R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator <(Hash256 L, Hash256 R) => L.CompareTo(R) < 0;

        /// <summary>
        /// L > R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator >(Hash256 L, Hash256 R) => L.CompareTo(R) > 0;

        /// <summary>
        /// L <= R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator <=(Hash256 L, string R) => L.CompareTo(R) <= 0;

        /// <summary>
        /// L >= R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator >=(Hash256 L, string R) => L.CompareTo(R) >= 0;

        /// <summary>
        /// L < R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator <(Hash256 L, string R) => L.CompareTo(R) < 0;

        /// <summary>
        /// L > R.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool operator >(Hash256 L, string R) => L.CompareTo(R) > 0;

        /// <summary>
        /// Compute `NOT`.
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Hash256 operator ~(Hash256 Value)
        {
            var Temp = new byte[Size];
            for (var i = 0; i < Size; i++)
                Temp[i] = (byte)~Value.Raw[i];

            return new Hash256(Temp);
        }

        /// <summary>
        /// Compute XOR.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static Hash256 operator ^(Hash256 L, Hash256 R)
        {
            var Temp = new byte[Size];
            for (var i = 0; i < Size; i++)
                Temp[i] = (byte)(L.Raw[i] ^ R.Raw[i]);

            return new Hash256(Temp);
        }

        /// <summary>
        /// Compute AND.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static Hash256 operator &(Hash256 L, Hash256 R)
        {
            var Temp = new byte[Size];
            for (var i = 0; i < Size; i++)
                Temp[i] = (byte)(L.Raw[i] & R.Raw[i]);

            return new Hash256(Temp);
        }

        /// <summary>
        /// Compute OR.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static Hash256 operator |(Hash256 L, Hash256 R)
        {
            var Temp = new byte[Size];
            for (var i = 0; i < Size; i++)
                Temp[i] = (byte)(L.Raw[i] | R.Raw[i]);

            return new Hash256(Temp);
        }

    }
}