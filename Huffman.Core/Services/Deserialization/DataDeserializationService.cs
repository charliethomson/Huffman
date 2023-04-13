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
        var buffer = new List<int>(data.Count * 2);

        var currentIndex = 1;

        while (currentOffset < data.Count)
        {
            var currentNode = nodes[currentIndex - 1];
            if (bitOffset < 0)
            {
                bitOffset = 7;
                if (++currentOffset >= data.Count) break;

                section = data[currentOffset];
            }

            if (currentNode.Value != 0)
            {
                buffer.Add(currentNode.Value);
                currentIndex = 1;
                currentNode = nodes[0];
            }

            var isLastByte = currentOffset == data.Count - 1;
            var hasLastBytePadding = lastBytePadding != 0;
            if (isLastByte && hasLastBytePadding && bitOffset == lastBytePadding - 1) break;

            var mask = (0b1u << bitOffset);
            var maskBit = (section & mask);

            var bit = maskBit >> bitOffset;

            currentIndex = bit == 1 ? currentNode.RightIndex : currentNode.LeftIndex;
            bitOffset--;
        }

        var currentValue = nodes[currentIndex - 1].Value;
        if (currentValue != 0)
            buffer.Add(currentValue);

        return string.Join("", buffer.Select(Convert.ToChar));
    }
}