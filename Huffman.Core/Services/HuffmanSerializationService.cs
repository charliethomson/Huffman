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
    private readonly IConfiguration _configuration;
    private readonly ITreeGenerationService _treeGenerationService;
    private readonly ITreeSerializationService _treeSerializationService;
    private readonly IDataSerializationService _dataSerializationService;

    private uint MagicBytes => Convert.ToUInt32(_configuration["Serde:MagicBytes"]);

    public HuffmanSerializationService(
        IConfiguration configuration,
        IDataSerializationService dataSerializationService,
        ITreeSerializationService treeSerializationService,
        ITreeGenerationService treeGenerationService)
    {
        _configuration =
            configuration ?? throw new ArgumentNullException(nameof(configuration));
        _treeSerializationService =
            treeSerializationService ?? throw new ArgumentNullException(nameof(treeSerializationService));
        _treeGenerationService =
            treeGenerationService ?? throw new ArgumentNullException(nameof(treeGenerationService));
        _dataSerializationService =
            dataSerializationService ?? throw new ArgumentNullException(nameof(dataSerializationService));
    }

    public IEnumerable<byte> Serialize(string data)
    {
        var tree = _treeGenerationService.GenerateHuffmanTree(data);

        var (encodedData, lastBytePaddingAmount) = _dataSerializationService.SerializeData(data, tree);
        var serializedTree = _treeSerializationService.SerializeTree(tree).ToList();


        var bytes = new List<byte>();
        bytes.AddRange(BitConverter.GetBytes(MagicBytes));
        bytes.AddRange(BitConverter.GetBytes(serializedTree.Count));
        bytes.AddRange(serializedTree);
        bytes.AddRange(BitConverter.GetBytes(encodedData.Count));
        bytes.Add(lastBytePaddingAmount);
        bytes.AddRange(encodedData);
        return bytes;
    }
}