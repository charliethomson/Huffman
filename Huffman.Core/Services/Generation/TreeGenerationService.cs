using Huffman.Infra.Services.Generation;
using Huffman.Models;

namespace Huffman.Core.Services.Generation;

public class TreeGenerationService : ITreeGenerationService
{
    public TreeNode GenerateHuffmanTree(string data)
    {
        var treeNodes = new TreeNode?[255];

        foreach (var c in data)
        {
            var b = (byte)c;
            treeNodes[b] ??= new TreeNode(c, 0);
            treeNodes[b]!.IntrinsicValue++;
        }

        TreeNode[] nonZeroNodes = treeNodes.Where(node => node != null).ToArray()!;

        var queue = new PriorityQueue<TreeNode, uint>(nonZeroNodes.Length);
        foreach (var node in nonZeroNodes)
            queue.Enqueue(node, node.Value);

        // TODO: Error checking
        while (queue.Count > 1)
        {
            var node1 = queue.Dequeue();
            var node2 = queue.Dequeue();

            var newNode = new TreeNode(node1, node2);
            queue.Enqueue(newNode, newNode.Value);
        }

        return queue.Dequeue();
    }
}