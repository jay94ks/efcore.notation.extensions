namespace Efcore.Notation.Extensions.Utilities.Ecdsa
{

    /// <summary>
    /// Helpers.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Read <see cref="Hash256"/>.
        /// </summary>
        /// <param name="Reader"></param>
        /// <returns></returns>
        public static Hash256 ReadHash256(this BinaryReader Reader)
        {
            return new Hash256(Reader.ReadBytes(Hash256.Size));
        }

        /// <summary>
        /// Read <see cref="Pub256"/>.
        /// </summary>
        /// <param name="Reader"></param>
        /// <returns></returns>
        public static Pub256 ReadPub256(this BinaryReader Reader)
        {
            return new Pub256(Reader.ReadBytes(Pub256.Size));
        }

        /// <summary>
        /// Read <see cref="Key256"/>.
        /// </summary>
        /// <param name="Reader"></param>
        /// <returns></returns>
        public static Key256 ReadKey256(this BinaryReader Reader)
        {
            return new Key256(Reader.ReadBytes(Key256.Size));
        }

        /// <summary>
        /// Read <see cref="Sign256"/>.
        /// </summary>
        /// <param name="Reader"></param>
        /// <returns></returns>
        public static Sign256 ReadSign256(this BinaryReader Reader)
        {
            return new Sign256(Reader.ReadBytes(Sign256.Size));
        }

        /// <summary>
        /// Write <see cref="Hash256"/>.
        /// </summary>
        /// <param name="Writer"></param>
        /// <param name="Hash256"></param>
        public static void Write(this BinaryWriter Writer, Hash256 Hash256)
        {
            Writer.Write(Hash256.Raw);
        }

        /// <summary>
        /// Write <see cref="Pub256"/>.
        /// </summary>
        /// <param name="Writer"></param>
        /// <param name="Pub256"></param>
        public static void Write(this BinaryWriter Writer, Pub256 Pub256)
        {
            Writer.Write(Pub256.Raw);
        }

        /// <summary>
        /// Write <see cref="Key256"/>.
        /// </summary>
        /// <param name="Writer"></param>
        /// <param name="Key256"></param>
        public static void Write(this BinaryWriter Writer, Key256 Key256)
        {
            Writer.Write(Key256.Raw);
        }

        /// <summary>
        /// Write <see cref="Sign256"/>.
        /// </summary>
        /// <param name="Writer"></param>
        /// <param name="Sign256"></param>
        public static void Write(this BinaryWriter Writer, Sign256 Sign256)
        {
            Writer.Write(Sign256.Raw);
        }
    }
}
