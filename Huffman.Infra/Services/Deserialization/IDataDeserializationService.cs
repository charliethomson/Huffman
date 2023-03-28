
using Huffman.Models;

namespace Huffman.Infra.Services.Deserialization;

public interface IDataDeserializationService
{
    string DeserializeData(IReadOnlyList<byte> data, IReadOnlyList<InternalTreeNode> nodes, byte lastBytePadding);
}