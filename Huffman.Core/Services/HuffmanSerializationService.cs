using Huffman.Core.Services.Generation;
using Huffman.Infra.Services;
using Huffman.Infra.Services.Deserialization;
using Huffman.Infra.Services.Generation;
using Huffman.Infra.Services.Serialization;
using Huffman.Services.Serde;
using Microsoft.Extensions.Configuration;

namespace Huffman.Core.Services;

public class HuffmanSerializationService : IHuffmanSerializationService
{
    private readonly ITreeGenerationService _treeGenerationService;
    private readonly ITreeSerializationService _treeSerializationService;
    private readonly IDataSerializationService _dataSerializationService;

    private readonly uint? _magicBytes;
    public HuffmanSerializationService(
        IConfiguration configuration,
        IDataSerializationService dataSerializationService,
        ITreeSerializationService treeSerializationService,
        ITreeGenerationService treeGenerationService)
    {
        _treeSerializationService =
            treeSerializationService ?? throw new ArgumentNullException(nameof(treeSerializationService));
        _treeGenerationService =
            treeGenerationService ?? throw new ArgumentNullException(nameof(treeGenerationService));
        _dataSerializationService =
            dataSerializationService ?? throw new ArgumentNullException(nameof(dataSerializationService));
        _magicBytes ??= Convert.ToUInt32(configuration["Serde:MagicBytes"]);
    }

    public IEnumerable<byte> Serialize(string data)
    {
        var tree = _treeGenerationService.GenerateHuffmanTree(data);

        var (encodedData, lastBytePaddingAmount) = _dataSerializationService.SerializeData(data, tree);
        var serializedTree = _treeSerializationService.SerializeTree(tree).ToList();


        var bytes = new List<byte>();
        bytes.AddRange(BitConverter.GetBytes(_magicBytes!.Value));
        bytes.AddRange(BitConverter.GetBytes(serializedTree.Count));
        bytes.AddRange(serializedTree);
        bytes.AddRange(BitConverter.GetBytes(encodedData.Count));
        bytes.Add(lastBytePaddingAmount);
        bytes.AddRange(encodedData);
        return bytes;
    }
}