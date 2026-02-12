using UnityEngine;

/// <summary>
/// Represents a room node in the dungeon structure hierarchy.
/// Extends the base Node class and calculates room dimensions based on corner coordinates.
/// </summary>
public class RoomNode : Node
{
    /// <summary>
    /// Constructor to initialize a room node with specified corners and position in the tree
    /// </summary>
    /// <param name="bottomLeftAreaCorner">The bottom-left corner coordinate of the room</param>
    /// <param name="topRightAreaCorner">The top-right corner coordinate of the room</param>
    /// <param name="parentNode">The parent node in the tree hierarchy</param>
    /// <param name="index">The tree layer index for this room</param>
    public RoomNode(Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, Node parentNode, int index) : base(parentNode)
    {
        this.BottomLeftAreaCorner = bottomLeftAreaCorner;
        this.TopRightAreaCorner = topRightAreaCorner;
        this.BottomRightAreaCorner = new Vector2Int(topRightAreaCorner.x, bottomLeftAreaCorner.y);
        this.TopLeftAreaCorner = new Vector2Int(bottomLeftAreaCorner.x, topRightAreaCorner.y);
        this.treeLayerIndex = index;
    }

    /// <summary> Calculated width of the room based on x-coordinates of corners </summary>
    public int width {get => (int)(TopRightAreaCorner.x - BottomLeftAreaCorner.x);}
    
    /// <summary> Calculated length/height of the room based on y-coordinates of corners </summary>
    public int length {get => (int)(TopRightAreaCorner.y - BottomLeftAreaCorner.y);}
}
