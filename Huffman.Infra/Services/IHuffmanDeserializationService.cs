namespace Huffman.Infra;

public interface IHuffmanDeserializationService
{
    string Deserialize(byte[] bytes);
}