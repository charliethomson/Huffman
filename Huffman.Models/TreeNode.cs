using System.Text.RegularExpressions;

namespace Huffman.Models;

public class TreeNode
{
    private readonly uint _intrinsicValue = 0;
    public Guid Id { get; }
    public char? Item { get; set; } = null;

    public TreeNode? Parent { get; set; }

    private TreeNode? _left;
    private TreeNode? _right;


    public TreeNode? Left
    {
        get => _left;
        init
        {
            _left = value;
            if (value != null) value.Parent = this;
        }
    }

    public TreeNode? Right
    {
        get => _right;
        init
        {
            _right = value;
            if (value != null) value.Parent = this;
        }
    }

    public uint Value
    {
        get
        {
            if (_intrinsicValue != 0) return _intrinsicValue;

            return (Left?.Value ?? 0) + (Right?.Value ?? 0);
        }
    }

    public TreeNode()
    {
        Id = Guid.NewGuid();
    }

    public TreeNode(char item) : this()
    {
        Item = item;
    }

    public TreeNode(char item, uint intrinsicValue) : this(item)
    {
        _intrinsicValue = intrinsicValue;
    }

    public TreeNode(TreeNode left) : this()
    {
        Left = left;
        Left.Parent = this;
    }

    public TreeNode(TreeNode left, TreeNode right) : this(left)
    {
        Right = right;
        Right.Parent = this;
    }

    public bool AmIRightChild()
    {
        return Parent?.Right != null && Parent.Right.Id == Id;
    }

    public (uint path, int depth) GetPathFromRoot()
    {
        if (Parent == null) return (0, 0);
        var amIRight = AmIRightChild();

        var (path, depth) = Parent.GetPathFromRoot();

        path <<= 1;
        path |= Convert.ToUInt32(amIRight);
        return (path, depth + 1);
    }

    public bool TryFindChildWithItem(char item, out TreeNode node)
    {
        node = null!;

        if (Item == item)
        {
            node = this;
            return true;
        }

        if (Left?.TryFindChildWithItem(item, out node) ?? false) return true;
        if (Right?.TryFindChildWithItem(item, out node) ?? false) return true;

        return false;
    }

    public IEnumerable<TreeNode> GetAllChildren()
    {
        var children = new List<TreeNode>();
        
        if (Left != null)
        {
            children.Add(Left);
            children.AddRange(Left.GetAllChildren());
        }

        if (Right != null)
        {
            children.Add(Right);
            children.AddRange(Right.GetAllChildren());
        }

        return children;
    }

    public void PrintPretty(string indent, bool last)
    {
        Console.Write(indent);
        if (last)
        {
            Console.Write("\\-");
            indent += "  ";
        }
        else
        {
            Console.Write("|-");
            indent += "| ";
        }

        if (Item != null)
        {
            var (path, depth) = GetPathFromRoot();
            Console.WriteLine(
                $@"Id={Id};Item='{Regex.Escape(Item.ToString())}';_intrinsicValue={_intrinsicValue};Path={Convert.ToString(path, 2)};Depth={depth};");
        }
        else
            Console.WriteLine(
                $@"Id={Id};Value={Value};");

        Left?.PrintPretty(indent, Right == null);
        Right?.PrintPretty(indent, true);
    }

    public IEnumerable<byte> Serialize(Func<Guid, byte> nodeIndexLookup)
    {
        var leftIndex = (byte)(Left == null ? 0 : nodeIndexLookup(Left.Id));
        var rightIndex = (byte)(Right == null ? 0 : nodeIndexLookup(Right.Id));
        var item = Item == null ? (byte)0 : (byte)Item;

        return new byte[] { leftIndex, rightIndex, item };
    }
}