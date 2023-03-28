using Huffman.Infra.Services.Serialization;
using Huffman.Models;

namespace Huffman.Core.Services.Serialization;

public class TreeSerializationService : ITreeSerializationService
{
    public IEnumerable<byte> SerializeTree(TreeNode root)
    {
        var allNodes = root.GetAllChildren().ToList();
        allNodes.Insert(0, root);

        return allNodes.SelectMany(
            node => node.Serialize(nodeId => (byte)(allNodes.FindIndex(treeNode => treeNode.Id == nodeId) + 1)));
    }
}