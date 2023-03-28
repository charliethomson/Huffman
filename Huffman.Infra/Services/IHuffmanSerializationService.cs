namespace Huffman.Infra.Services;

public interface IHuffmanSerializationService
{
    IEnumerable<byte> Serialize(string data);
}