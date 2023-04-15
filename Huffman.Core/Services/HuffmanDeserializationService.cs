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

    private static byte ReadByte(byte[] data, ref int offset)
    {
        var b = data[offset];
        offset += 1;
        return b;
    }

    private static int ReadInt(byte[] data, ref int offset)
    {
        var i = BitConverter.ToInt32(new Span<byte>(data, offset, 4));
        offset += 4;
        return i;
    }

    private static Span<byte> ReadBytes(byte[] data, ref int offset, int size)
    {
        var bytes = new Span<byte>(data, offset, size);
        offset += size;
        return bytes;
    }

    public string Deserialize(byte[] bytes)
    {
        var offset = 0;
        var magic = ReadInt(bytes, ref offset);
        if (magic != MagicBytes) throw new ArgumentException("This doesnt seem to be HEC data");

        var treeSize = ReadInt(bytes, ref offset);
        var treeData = ReadBytes(bytes, ref offset, treeSize);

        var dataSize = ReadInt(bytes, ref offset);
        var dataEndPadding = ReadByte(bytes, ref offset);
        var data = ReadBytes(bytes, ref offset, dataSize);

        var tree = _treeDeserializationService.DeserializeTree(treeData);
        return _dataDeserializationService.DeserializeData(data, tree.ToArray(), dataEndPadding);
    }
}