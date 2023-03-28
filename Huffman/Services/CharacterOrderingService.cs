// using Huffman.Models;
//
// namespace Huffman.Services;
//
// public class CharacterOrderingService
// {
//     public static IDictionary<char, uint> GetCharacterCounts(string buffer)
//     {
//     }
//
//     public static (List<byte> bytes, byte lastBytePaddingAmount) EncodeString(string buffer, TreeNode tree)
//     {
//         const int MaxSize = 8;
//         var lookupByChar = new Dictionary<char, TreeNode>();
//         var result = new List<byte>();
//         byte workingByte = 0;
//         var workingOffset = 0;
//
//         foreach (var c in buffer)
//         {
//             if (lookupByChar.TryGetValue(c, out var node)) ;
//             else if (tree.TryFindChildWithItem(c, out node))
//             {
//                 lookupByChar.Add(c, node);
//             }
//             else throw new Exception($"Failed to find node with character {c}");
//
//             var (bitPath, bits) = node.GetPathFromRoot();
//
//             while (bits > 0)
//             {
//                 if (workingOffset >= MaxSize)
//                 {
//                     result.Add(workingByte);
//                     workingOffset = 0;
//                     workingByte = 0;
//                 }
//
//                 workingOffset++;
//
//                 workingByte <<= 1;
//                 var bitIdx = bits - 1;
//                 var bit = (byte)((bitPath & (0b1 << bitIdx)) >> bitIdx);
//                 workingByte |= bit;
//                 bits--;
//             }
//
//         }
//
//         if (workingByte != 0) result.Add((byte)(workingByte << (8 - workingOffset)));
//
//         return (result, (byte)(8 - workingOffset));
//     }
// }
// // 1010001010100101000101010001010
// // 1010001111100111000111010001111