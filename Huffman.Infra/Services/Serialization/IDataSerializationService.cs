using Huffman.Models;

namespace Huffman.Infra.Services.Serialization;

public interface IDataSerializationService
{
    (ICollection<byte> encodedData, byte lastBytePaddingAmount) SerializeData(string data, TreeNode tree);
}