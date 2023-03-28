using Huffman.Models;

namespace Huffman.Infra.Services.Generation;

public interface ITreeGenerationService
{
    TreeNode GenerateHuffmanTree(string data);
}