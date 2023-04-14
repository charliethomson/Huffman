using System.Diagnostics;
using Huffman.Infra.Services.Serialization;
using Huffman.Models;

namespace Huffman.Core.Services.Serialization;

public class DataSerializationService : IDataSerializationService
{
    private const int WorkingIntSize = 8;

    public (ICollection<byte> encodedData, byte lastBytePaddingAmount) SerializeData(string data, TreeNode tree)
    {
        var lookupBitsArr = new (uint, int)?[255];
        var result = new List<byte>(data.Length);
        byte workingByte = 0;
        var workingOffset = 0;

        for (var index = 0; index < data.Length; index++)
        {
            var c = data[index];
            uint bitPath;
            int bits;
            var existingEntry = lookupBitsArr[(byte)c];
            if (existingEntry.HasValue)
            {
                (bitPath, bits) = existingEntry.Value;
            }
            else if (tree.TryFindChildWithItem(c, out var node))
            {
                (bitPath, bits) = node.GetPathFromRoot();
                lookupBitsArr[(byte)c] = (bitPath, bits);
            }
            else throw new UnreachableException();

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

        var remainder = 8 - workingOffset;
        switch (remainder)
        {
            case 0:
                result.Add(workingByte);
                return (result, 0);
            case WorkingIntSize:
                return (result, (byte)remainder);
        }

        var lastByte = workingByte << remainder;
        // Fill remainder bits - 4 => 0b00010000 - 1 => 0b00001111
        var padding = (0b1 << remainder) - 1;
        result.Add((byte)(lastByte | padding));
        return (result, (byte)remainder);
    }
}