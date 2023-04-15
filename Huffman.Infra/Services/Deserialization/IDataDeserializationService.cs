
using Huffman.Models;

namespace Huffman.Infra.Services.Deserialization;

public interface IDataDeserializationService
{
    string DeserializeData(Span<byte> data, InternalTreeNode[] nodes, byte lastBytePadding);
}