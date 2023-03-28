namespace Huffman.Infra.Services;

public interface IHuffmanService
{
    string DeserializeData(IEnumerable<byte> data);
    string DeserializeFile(string filePath, string outPath);

    ICollection<byte> SerializeData(string data);
    ICollection<byte> SerializeFile(string filePath, string outPath);

    string ReadEncodedFile(string filePath);
    string WriteToFile(string data, string outPath);
}