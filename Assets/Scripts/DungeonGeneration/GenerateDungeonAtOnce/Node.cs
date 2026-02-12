using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract base class representing a node in the dungeon structure tree.
/// Used for both rooms and corridors in the binary space partitioning system.
/// </summary>
public abstract class Node
{
    /// <summary> List of child nodes in the tree hierarchy </summary>
    private List<Node> childrenNodeList;

    /// <summary> List of child nodes in the tree hierarchy </summary>
    public List<Node> ChildrenNodeList { get => childrenNodeList;}
    
    /// <summary> Flag to track if this node has been visited during traversal </summary>
    public bool Visted { get; set; }
    
    /// <summary> The bottom-left corner coordinate of this node's area </summary>
    public Vector2Int BottomLeftAreaCorner { get; set; }
    
    /// <summary> The bottom-right corner coordinate of this node's area </summary>
    public Vector2Int BottomRightAreaCorner { get; set; }
    
    /// <summary> The top-left corner coordinate of this node's area </summary>
    public Vector2Int TopLeftAreaCorner { get; set; }
    
    /// <summary> The top-right corner coordinate of this node's area </summary>
    public Vector2Int TopRightAreaCorner { get; set; }

    /// <summary> Reference to the parent node in the tree hierarchy </summary>
    public Node parentNode { get; set; }

    /// <summary> The depth level of this node in the binary space partition tree </summary>
    public int treeLayerIndex { get; set; }

    /// <summary>
    /// Constructor to initialize a node with an optional parent node
    /// </summary>
    /// <param name="parentNode">The parent node, or null if this is the root</param>
    public Node(Node parentNode)
    {
        childrenNodeList = new List<Node>();
        this.parentNode = parentNode;

        // Automatically add this node as a child to its parent
        if(parentNode != null)
            parentNode.AddChild(this);
    }

    /// <summary>
    /// Adds a child node to this node's children list
    /// </summary>
    /// <param name="childNode">The child node to add</param>
    public void AddChild(Node childNode)
    {
        childrenNodeList.Add(childNode);
    }

    /// <summary>
    /// Removes a child node from this node's children list
    /// </summary>
    /// <param name="childNode">The child node to remove</param>
    public void RemoveChild(Node childNode)
    {
        childrenNodeList.Remove(childNode);
    }
}
