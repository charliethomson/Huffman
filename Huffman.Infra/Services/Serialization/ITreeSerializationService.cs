
using Huffman.Models;

namespace Huffman.Infra.Services.Serialization;

public interface ITreeSerializationService
{
    IEnumerable<byte> SerializeTree(TreeNode root);
}