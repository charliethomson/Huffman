using Huffman.Infra.Services.Serialization;
using Huffman.Models;

namespace Huffman.Core.Services.Serialization;

public class DataSerializationService : IDataSerializationService
{
    private const int WorkingIntSize = 8;

    public (ICollection<byte> encodedData, byte lastBytePaddingAmount) SerializeData(string data, TreeNode tree)
    {
        var lookupByChar = new Dictionary<char, TreeNode>();
        var result = new List<byte>();
        byte workingByte = 0;
        var workingOffset = 0;

        foreach (var c in data)
        {
            if (lookupByChar.TryGetValue(c, out var node)) ;
            else if (tree.TryFindChildWithItem(c, out node))
            {
                lookupByChar.Add(c, node);
            }
            else throw new Exception($"Failed to find node with character {c}");

            var (bitPath, bits) = node.GetPathFromRoot();

            while (bits > 0)
            {
                if (workingOffset >= WorkingIntSize)
                {
                    result.Add(workingByte);
                    workingOffset = 0;
                    workingByte = 0;
                }

                workingOffset++;

                workingByte <<= 1;
                var bitIdx = bits - 1;
                var bit = (byte)((bitPath & (0b1 << bitIdx)) >> bitIdx);
                workingByte |= bit;
                bits--;
            }
        }

        if (workingByte != 0) result.Add((byte)(workingByte << (8 - workingOffset)));

        return (result, (byte)(8 - workingOffset));
    }
}