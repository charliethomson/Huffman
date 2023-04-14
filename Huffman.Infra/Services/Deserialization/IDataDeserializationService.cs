
using Huffman.Models;

namespace Huffman.Infra.Services.Deserialization;

public interface IDataDeserializationService
{
    string DeserializeData(byte[] data, InternalTreeNode[] nodes, byte lastBytePadding);
}