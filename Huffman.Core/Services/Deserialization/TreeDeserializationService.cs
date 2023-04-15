using Huffman.Models;
using Huffman.Services.Serde;

namespace Huffman.Core.Services.Deserialization;

public class TreeDeserializationService : ITreeDeserializationService
{
    public InternalTreeNode[] DeserializeTree(Span<byte> bytes)
    {
        var nodes = new InternalTreeNode[bytes.Length / 3];

        for (var i = 0; i < nodes.Length; i++)
        {
            var leftIndex = bytes[i * 3];
            var rightIndex = bytes[i * 3 + 1];
            var value = bytes[i * 3 + 2];

            nodes[i] = new InternalTreeNode() { Value = value, LeftIndex = leftIndex, RightIndex = rightIndex };
        }

        return nodes;
    }
}