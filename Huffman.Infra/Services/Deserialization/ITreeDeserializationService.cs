using Huffman.Models;

namespace Huffman.Services.Serde;

public interface ITreeDeserializationService
{
    List<InternalTreeNode> DeserializeTree(IEnumerable<byte> bytes);
}