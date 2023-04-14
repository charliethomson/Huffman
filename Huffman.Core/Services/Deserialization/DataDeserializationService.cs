using System.Text;
using Huffman.Infra.Services.Deserialization;
using Huffman.Models;

namespace Huffman.Core.Services.Deserialization;

public class DataDeserializationService : IDataDeserializationService
{
    public string DeserializeData(byte[] data, InternalTreeNode[] nodes, byte lastBytePadding)
    {
        var bitOffset = 7;
        var currentOffset = 0;

        var section = data[currentOffset];
        var dataCount = data.Length;
        var sb = new StringBuilder(dataCount * 2);

        var currentIndex = 1;

        while (currentOffset < dataCount)
        {
            var currentNode = nodes[currentIndex - 1];
            if (bitOffset < 0)
            {
                bitOffset = 7;
                if (++currentOffset >= dataCount) break;

                section = data[currentOffset];
            }

            if (currentNode.Value != 0)
            {
                sb.Append((char)currentNode.Value);
                currentIndex = 1;
                currentNode = nodes[0];
            }

            var isLastByte = currentOffset == dataCount - 1;
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
            sb.Append((char)currentValue);

        return sb.ToString();
    }
}