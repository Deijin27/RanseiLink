#nullable enable
using System.Collections.Generic;

namespace RanseiLink.Core.Graphics;

public class RadixTreeGenerator
{
    private List<RadixNode> _nodes = new List<RadixNode>();

    private RadixTreeGenerator()
    {
        AddRootNode();
    }

    private void AddRootNode()
    {
        RadixNode patTreeNode = new RadixNode(name: new string('\0', 16))
        {
            Refbit = sbyte.MaxValue,
            IdxEntry = 0,
            
        };
        patTreeNode.Left = patTreeNode;
        patTreeNode.Right = patTreeNode;
        
        _nodes.Add(patTreeNode);
    }

    RadixNode RootNode => _nodes[0];

    private RadixNode? AddNode(string nodeName, int entryIndex)
    {
        // name must be 16 long
        nodeName = nodeName.PadRight(16, '\0');

        // prevent duplicates
        foreach (RadixNode node in _nodes)
        {
            if (node.Name == nodeName)
            {
                return null;
            }
        }

        RadixNode currentNode = RootNode.Left;
        if (RootNode.Refbit > RootNode.Left.Refbit)
        {
            int refbit = RootNode.Left.Refbit;
            RadixNode previousNode;
            do
            {
                var stringPart = GetStringPart(nodeName, (refbit >> 5 & 3) * 4);
                previousNode = currentNode;
                currentNode = (stringPart >> refbit & 1) == 0 ? previousNode.Left : previousNode.Right;
                refbit = currentNode.Refbit;
            }
            while (previousNode.Refbit > refbit);
        }

        int maxValue = sbyte.MaxValue;
        if ((currentNode.IdxEntry ^ GetStringPart(nodeName, 12)) >= 0)
        {
            int num2;
            do
            {
                --maxValue;
                num2 = maxValue >> 5 & 3;
            }
            while (((GetStringPart(currentNode.Name, num2 * 4) ^ GetStringPart(nodeName, num2 * 4)) >> (maxValue & 31 & byte.MaxValue) & 1) == 0);
        }

        RadixNode patTreeNode5 = RootNode.Left;
        RadixNode patTreeNode6 = RootNode;
        int refbit1 = RootNode.Left.Refbit;
        if (RootNode.Refbit > RootNode.Left.Refbit)
        {
            while (refbit1 > maxValue)
            {
                patTreeNode6 = patTreeNode5;
                patTreeNode5 = (GetStringPart(nodeName, (refbit1 >> 5 & 3) * 4) >> refbit1 & 1) == 0 ? patTreeNode5.Left : patTreeNode5.Right;
                refbit1 = patTreeNode5.Refbit;
                if (patTreeNode6.Refbit <= refbit1)
                {
                    break;
                }
            }
        }

        RadixNode newNode = new RadixNode(name: nodeName)
        {
            Refbit = maxValue,
            IdxEntry = entryIndex
        };
        newNode.Left = (GetStringPart(nodeName, (maxValue >> 5 & 3) * 4) >> maxValue & 1) != 0 ? patTreeNode5 : newNode;
        newNode.Right = (GetStringPart(nodeName, (maxValue >> 5 & 3) * 4) >> maxValue & 1) != 0 ? newNode : patTreeNode5;
        if ((GetStringPart(nodeName, (patTreeNode6.Refbit >> 5 & 3) * 4) >> patTreeNode6.Refbit & 1) != 0)
        {
            patTreeNode6.Right = newNode;
        }
        else
        {
            patTreeNode6.Left = newNode;
        }

        // add the newly created node to the list
        _nodes.Add(newNode);

        return newNode;
    }

    /// <summary>
    /// Get an integer of the values of the 4 chars starting from the offset
    /// e.g. 'hello' is <code>68 65 6C 6C 6F</code>, which if we let offset be 1, gives the number 0x6F6C6C65
    /// </summary>
    private int GetStringPart(string str, int offset)
    {
        int stringPart = 0;
        for (int i = 0; i < 4; ++i)
        {
            stringPart |= str[offset + i] << i * 8;
        }

        return stringPart;
    }

    /// <summary>
    /// Sort the nodes correctly by walking the tree.
    /// </summary>
    private void Sort()
    {
        var root = RootNode;
        _nodes.Clear();
        SortInternal(root);
    }

    private void SortInternal(RadixNode node)
    {
        if (!_nodes.Contains(node))
        {
            _nodes.Add(node);
            if (node.Left != node)
            {
                SortInternal(node.Left);
            }
            if (node.Right != node)
            {
                SortInternal(node.Right);
            }
        }
    }

    private RawRadixNode[] GetRawNodes()
    {
        List<RawRadixNode> ptreeNodeList = new List<RawRadixNode>();
        foreach (RadixNode node in _nodes)
        {
            ptreeNodeList.Add(new RawRadixNode(
                refBit: (byte)node.Refbit,
                idxLeft: (byte)_nodes.IndexOf(node.Left),
                idxRight: (byte)_nodes.IndexOf(node.Right),
                idxEntry: (byte)node.IdxEntry
            ));
        }

        return ptreeNodeList.ToArray();
    }

    public static RawRadixNode[] Generate(string[] names)
    {
        RadixTreeGenerator patriciaTreeGenerator = new RadixTreeGenerator();
        for (int Index = 0; Index < names.Length; ++Index)
        {
            patriciaTreeGenerator.AddNode(names[Index], Index);
        }

        patriciaTreeGenerator.Sort();
        return patriciaTreeGenerator.GetRawNodes();
    }

    private class RadixNode
    {
        public RadixNode(string name)
        {
            Name = name;
        }
        public int Refbit { get; set; }
        public RadixNode Left { get; set; } = null!;
        public RadixNode Right { get; set; } = null!;
        public int IdxEntry { get; set; }
        public string Name { get; set; }
    }
}