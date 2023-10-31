namespace Efcore.Notation.Extensions.Utilities
{
    /// <summary>
    /// Binary Serializable interface.
    /// </summary>
    public interface IBinarySerializable
    {
        /// <summary>
        /// Serialize this object to the binary writer.
        /// </summary>
        /// <param name="Writer"></param>
        void Serialize(BinaryWriter Writer);

        /// <summary>
        /// Deserialize this object from the binary reader.
        /// </summary>
        /// <param name="Reader"></param>
        void Deserialize(BinaryReader Reader);
    }
}
