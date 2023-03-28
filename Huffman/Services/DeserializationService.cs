// using Huffman.Models;
//
// namespace Huffman.Services;
//
// public class DeserializationService
// {
//     private const uint Magic = 0x24484543;
//
//     private static uint ReadU32(ref List<byte> bytes)
//     {
//         var data = BitConverter.ToUInt32(bytes.Take(4).ToArray());
//         bytes.RemoveRange(0, 4);
//         return data;
//     }
//
//     
//
//
//     private static List<InternalTreeNode> DeserializeTree(IEnumerable<byte> bytes)
//     {
//     }
//
//     private static string DeserializeData(IReadOnlyList<byte> data, IReadOnlyList<InternalTreeNode> nodes, byte lastBytePadding)
//     {
//         var bitOffset = 7;
//         var currentOffset = 0;
//
//         var section = data[currentOffset];
//
//         var currentIndex = 1;
//
//         InternalTreeNode CurrentNode() => nodes[currentIndex - 1];
//
//         var content = "";
//
//
//         while (currentOffset < data.Count)
//         {
//             if (bitOffset < 0)
//             {
//                 bitOffset = 7;
//                 if (++currentOffset >= data.Count) break;
//
//                 section = data[currentOffset];
//             }
//
//             if (CurrentNode().Value != 0)
//             {
//                 content += Convert.ToChar(CurrentNode().Value);
//                 currentIndex = 1;
//             }
//
//             if (currentOffset == data.Count - 1 && bitOffset == lastBytePadding - 1) break;
//
//             var mask = (0b1u << bitOffset);
//             var maskBit = (section & mask);
//
//             var bit = maskBit >> bitOffset;
//
//             currentIndex = bit == 1 ? CurrentNode().RightIndex : CurrentNode().LeftIndex;
//             bitOffset--;
//         }
//
//         return content;
//     }
//
//     public static string DeserializeHuffman(List<byte> bytes)
//     {
//         var magic = BitConverter.ToUInt32(bytes.Take(4).Reverse().ToArray());
//         if (magic != Magic) throw new ArgumentException("This doesnt seem to be HEC data");
//         bytes.RemoveRange(0, 4);
//
//
//         var treeSize = ReadU32(ref bytes);
//         var treeData = ReadBytes(ref bytes, (int)treeSize);
//
//         var dataSize = ReadU32(ref bytes);
//         var dataEndPadding = ReadU8(ref bytes);
//         var data = ReadBytes(ref bytes, (int)dataSize);
//
//         var tree = DeserializeTree(treeData);
//         return DeserializeData(data, tree, dataEndPadding);
//     }
//
// }