using Huffman.Models;

namespace Huffman.Services.Serde;

public interface ITreeDeserializationService
{
    public InternalTreeNode[] DeserializeTree(Span<byte> bytes);
}