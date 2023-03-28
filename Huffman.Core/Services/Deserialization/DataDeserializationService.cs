using Huffman.Infra.Services.Deserialization;
using Huffman.Models;

namespace Huffman.Core.Services.Deserialization;

public class DataDeserializationService : IDataDeserializationService
{
    public string DeserializeData(IReadOnlyList<byte> data, IReadOnlyList<InternalTreeNode> nodes, byte lastBytePadding)
    {
        var bitOffset = 7;
        var currentOffset = 0;

        var section = data[currentOffset];

        var currentIndex = 1;

        InternalTreeNode CurrentNode() => nodes[currentIndex - 1];

        var content = "";


        while (currentOffset < data.Count)
        {
            if (bitOffset < 0)
            {
                bitOffset = 7;
                if (++currentOffset >= data.Count) break;

                section = data[currentOffset];
            }

            if (CurrentNode().Value != 0)
            {
                content += Convert.ToChar(CurrentNode().Value);
                currentIndex = 1;
            }

            if (currentOffset == data.Count - 1 && bitOffset == lastBytePadding - 1) break;

            var mask = (0b1u << bitOffset);
            var maskBit = (section & mask);

            var bit = maskBit >> bitOffset;

            currentIndex = bit == 1 ? CurrentNode().RightIndex : CurrentNode().LeftIndex;
            bitOffset--;
        }

        return content;
    }
}