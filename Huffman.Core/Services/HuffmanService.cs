using Huffman.Infra;
using Huffman.Infra.Services;
using Huffman.Infra.Services.Deserialization;
using Huffman.Services.Serde;
using Microsoft.Extensions.Configuration;

namespace Huffman.Core.Services;

public class HuffmanService : IHuffmanService
{
    private readonly IConfiguration _configuration;
    private readonly IHuffmanSerializationService _huffmanSerializationService;
    private readonly IHuffmanDeserializationService _huffmanDeserializationService;

    public HuffmanService(IConfiguration configuration, IHuffmanSerializationService huffmanSerializationService,
        IHuffmanDeserializationService huffmanDeserializationService)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _huffmanSerializationService = huffmanSerializationService ??
                                       throw new ArgumentNullException(nameof(huffmanSerializationService));
        _huffmanDeserializationService = huffmanDeserializationService ??
                                         throw new ArgumentNullException(nameof(huffmanDeserializationService));
    }

    public string DeserializeData(IEnumerable<byte> data)
    {
        return _huffmanDeserializationService.Deserialize(data.ToArray());
    }

    public string DeserializeFile(string filePath, string outPath)
    {
        throw new NotImplementedException();
    }

    public ICollection<byte> SerializeData(string data)
    {
        return _huffmanSerializationService.Serialize(data).ToList();
    }

    public ICollection<byte> SerializeFile(string filePath, string outPath)
    {
        throw new NotImplementedException();
    }

    public string ReadEncodedFile(string filePath)
    {
        throw new NotImplementedException();
    }

    public string WriteToFile(string data, string outPath)
    {
        throw new NotImplementedException();
    }
}