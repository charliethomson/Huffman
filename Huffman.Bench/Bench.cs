using System.Text;
using BenchmarkDotNet.Attributes;
using Huffman.Core.Services;
using Huffman.Core.Services.Deserialization;
using Huffman.Core.Services.Generation;
using Huffman.Core.Services.Serialization;
using Huffman.Infra;
using Huffman.Infra.Services;
using Huffman.Infra.Services.Deserialization;
using Huffman.Infra.Services.Generation;
using Huffman.Infra.Services.Serialization;
using Huffman.Models;
using Huffman.Services.Serde;
using Microsoft.Extensions.Configuration;

namespace Huffman.Bench;

[MemoryDiagnoser]
public class Bench
{
    private IDataSerializationService _dataSerializationService { get; set; }
    private ITreeSerializationService _treeSerializationService { get; set; }
    private IDataDeserializationService _dataDeserializationService { get; set; }
    private ITreeDeserializationService _treeDeserializationService { get; set; }
    private IHuffmanSerializationService _huffmanSerializationService { get; set; }
    private IHuffmanDeserializationService _huffmanDeserializationService { get; set; }
    private IHuffmanService _huffmanService { get; set; }
    private ITreeGenerationService _treeGenerationService { get; set; }

    private List<byte> _serializedBytes { get; set; }
    private List<byte> _serializedChars { get; set; }
    private byte[] _serializedTree { get; set; }

    private List<byte> _bytes { get; set; }
    private string _chars { get; set; }
    private TreeNode _tree { get; set; }

    [Params(
        1_000
        , 10_000
        , 100_000
        , 1_000_000
        , 10_000_000
        , 100_000_000
        , 1_000_000_000
    )]
    public int NumBytes = 1;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();

        _dataSerializationService = new DataSerializationService();
        _treeGenerationService = new TreeGenerationService();
        _treeSerializationService = new TreeSerializationService();
        _dataDeserializationService = new DataDeserializationService();
        _treeDeserializationService = new TreeDeserializationService();
        _huffmanSerializationService = new HuffmanSerializationService(configuration, _dataSerializationService,
            _treeSerializationService, _treeGenerationService);
        _huffmanDeserializationService = new HuffmanDeserializationService(configuration, _dataDeserializationService,
            _treeDeserializationService);
        _huffmanService =
            new HuffmanService(configuration, _huffmanSerializationService, _huffmanDeserializationService);

        _bytes = new List<byte>(NumBytes);
        var sb = new StringBuilder(NumBytes);

        for (var i = 0; i < NumBytes; i++)
        {
            _bytes.Add((byte)Random.Shared.Next(0, 255));
            sb.Append((char)Random.Shared.Next(65, 90)); /* ASCII uppercase */
        }

        _chars = sb.ToString();
        _serializedChars = _huffmanService.SerializeData(_chars).ToList();
        _tree = _treeGenerationService.GenerateHuffmanTree(_chars);
        _serializedTree = _treeSerializationService.SerializeTree(_tree).ToArray();
    }

    [Benchmark]
    public void BenchStringEncoding() => _huffmanService.SerializeData(_chars);

    [Benchmark]
    public void BenchStringDecoding() => _huffmanService.DeserializeData(_serializedChars);

    [Benchmark]
    public void BenchTreeGeneration() => _treeGenerationService.GenerateHuffmanTree(_chars);

    [Benchmark]
    public void BenchTreeSerialization() => _treeSerializationService.SerializeTree(_tree);

    [Benchmark]
    public void BenchTreeDeserialization() =>
        _treeDeserializationService.DeserializeTree(new Span<byte>(_serializedTree, 0, _serializedTree.Length));
}