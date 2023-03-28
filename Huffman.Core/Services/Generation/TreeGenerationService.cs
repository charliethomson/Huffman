using Huffman.Infra.Services.Generation;
using Huffman.Models;

namespace Huffman.Core.Services.Generation;

public class TreeGenerationService : ITreeGenerationService
{
    public TreeNode GenerateHuffmanTree(string data)
    {
        var counts = new Dictionary<char, uint>();

        foreach (var c in data)
            counts[c] = counts.GetValueOrDefault(c) + 1;

        var nodes = counts.Select(pair => new TreeNode(pair.Key, pair.Value));

        var queue = new PriorityQueue<TreeNode, uint>(counts.Count);
        foreach (var node in nodes)
            queue.Enqueue(node, node.Value);

        var keepGoing = queue.Count >= 2;
        while (keepGoing)
        {
            var node1 = queue.Dequeue();
            var node2 = queue.Dequeue();

            var newNode = new TreeNode(node1, node2);
            queue.Enqueue(newNode, newNode.Value);

            keepGoing = queue.Count >= 2;
        }

        return queue.Dequeue();
    }
}