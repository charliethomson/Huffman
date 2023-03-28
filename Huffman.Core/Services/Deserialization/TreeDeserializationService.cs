using Huffman.Models;
using Huffman.Services.Serde;

namespace Huffman.Core.Services.Deserialization;

public class TreeDeserializationService : ITreeDeserializationService
{
    public List<InternalTreeNode> DeserializeTree(IEnumerable<byte> bytes)
    {
        return bytes.Chunk(3).Select(nodeBytes =>
        {
            var leftIndex = nodeBytes[0];
            var rightIndex = nodeBytes[1];
            var value = nodeBytes[2];

            return new InternalTreeNode() { Value = value, LeftIndex = leftIndex, RightIndex = rightIndex };
        }).ToList();
    }
}