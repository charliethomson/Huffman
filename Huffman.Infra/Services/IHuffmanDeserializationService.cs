namespace Huffman.Infra;

public interface IHuffmanDeserializationService
{
    string Deserialize(List<byte> bytes);
}