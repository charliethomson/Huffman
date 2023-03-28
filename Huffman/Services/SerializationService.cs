//
// using Huffman.Models;
//
// namespace Huffman.Services;
//
// public class SerializationService
// {
//     private static readonly byte[] Magic = "$HEC"u8.ToArray();
//
//     private static IEnumerable<byte> SerializeTree(TreeNode root)
//     {
//     }
//
//     public static IEnumerable<byte> SerializeHuffman(string data)
//     {
//         var tree = TreeGenerationService.GenerateHuffmanTree(data);
//         var (encodedData, lastBytePaddingAmount) = CharacterOrderingService.EncodeString(data, tree);
//         var serializedTree = SerializeTree(tree).ToList();
//         
//
//         var bytes = new List<byte>();
//         bytes.AddRange(Magic);
//         bytes.AddRange(BitConverter.GetBytes(serializedTree.Count));
//         bytes.AddRange(serializedTree);
//         bytes.AddRange(BitConverter.GetBytes(encodedData.Count));
//         bytes.Add(lastBytePaddingAmount);
//         bytes.AddRange(encodedData);
//         return bytes;
//     }
// }