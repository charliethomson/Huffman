using Huffman.Infra;
using Huffman.Infra.Services.Deserialization;
using Huffman.Services.Serde;
using Microsoft.Extensions.Configuration;

namespace Huffman.Core.Services;

public class HuffmanDeserializationService : IHuffmanDeserializationService
{
    private readonly IConfiguration _configuration;
    private readonly ITreeDeserializationService _treeDeserializationService;
    private readonly IDataDeserializationService _dataDeserializationService;

    private uint MagicBytes => Convert.ToUInt32(_configuration["Serde:MagicBytes"]);

    public HuffmanDeserializationService(IConfiguration configuration,
        IDataDeserializationService dataDeserializationService, ITreeDeserializationService treeDeserializationService)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _treeDeserializationService = treeDeserializationService ??
                                      throw new ArgumentNullException(nameof(treeDeserializationService));
        _dataDeserializationService = dataDeserializationService ??
                                      throw new ArgumentNullException(nameof(dataDeserializationService));
    }

    private static byte ReadU8(ref List<byte> bytes)
    {
        var b = bytes[0];
        bytes.RemoveAt(0);
        return b;
    }

    private static uint ReadU32(ref List<byte> bytes)
    {
        var data = BitConverter.ToUInt32(bytes.GetRange(0, 4).ToArray());
        bytes.RemoveRange(0, 4);
        return data;
    }

    private static List<byte> ReadBytes(ref List<byte> bytes, int nBytes)
    {
        var data = bytes.GetRange(0, nBytes);
        bytes.RemoveRange(0, nBytes);
        return data;
    }

    public string Deserialize(List<byte> bytes)
    {
        var magic = BitConverter.ToUInt32(bytes.Take(4).ToArray());
        if (magic != MagicBytes) throw new ArgumentException("This doesnt seem to be HEC data");
        bytes.RemoveRange(0, 4);


        var treeSize = ReadU32(ref bytes);
        var treeData = ReadBytes(ref bytes, (int)treeSize);

        var dataSize = ReadU32(ref bytes);
        var dataEndPadding = ReadU8(ref bytes);
        var data = ReadBytes(ref bytes, (int)dataSize);

        var tree = _treeDeserializationService.DeserializeTree(treeData);
        return _dataDeserializationService.DeserializeData(data.ToArray(), tree.ToArray(), dataEndPadding);
    }
}