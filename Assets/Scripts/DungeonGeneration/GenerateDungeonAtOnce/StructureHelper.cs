using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Utility class containing helper methods for dungeon structure manipulation and traversal.
/// Provides methods for tree traversal, random point generation, and calculations.
/// </summary>
public static class StructureHelper
{
    /// <summary>
    /// Traverses a node tree from a parent and extracts all leaf nodes (nodes with no children)
    /// Uses breadth-first search to find all terminal nodes in the hierarchy
    /// </summary>
    /// <param name="parentNode">The root node to start traversal from</param>
    /// <returns>A list of all leaf nodes (nodes with no children)</returns>
    public static List<Node> TraverseGraphToExtractLowestLeafs(Node parentNode)
    {
        Queue<Node> nodesToCheck = new Queue<Node>();
        List<Node> listToReturn = new List<Node>();

        // If the parent node itself has no children, it's already a leaf
        if(parentNode.ChildrenNodeList.Count == 0)
        {
            return new List<Node>() { parentNode };
        }
        
        // Queue all child nodes for processing
        foreach(var child in parentNode.ChildrenNodeList)
        {
            nodesToCheck.Enqueue(child);
        }

        // Process nodes until all are checked
        while(nodesToCheck.Count > 0)
        {
            var currentNode = nodesToCheck.Dequeue();
            if(currentNode.ChildrenNodeList.Count == 0)
            {
                // This is a leaf node, add it to results
                listToReturn.Add(currentNode);
            }
            else
            {
                // This node has children, queue them for processing
                foreach(var child in currentNode.ChildrenNodeList)
                {
                    nodesToCheck.Enqueue(child);
                }
            }
        }
        return listToReturn;
    }

    /// <summary>
    /// Generates a random bottom-left corner coordinate within a boundary with an offset and modifier
    /// Used to create variation in room placement within partition spaces
    /// </summary>
    /// <param name="boundaryLeftPoint">The minimum corner of the boundary area</param>
    /// <param name="boundaryRightPoint">The maximum corner of the boundary area</param>
    /// <param name="pointModifier">A percentage (0-1) to limit how far into the boundary the point can be</param>
    /// <param name="offset">Distance to inset from the boundary edges</param>
    /// <returns>A random point within the specified range</returns>
    public static Vector2Int GenerateBottomLeftCornerBetween(
        Vector2Int boundaryLeftPoint, Vector2Int boundaryRightPoint, float pointModifier, int offset)
    {
        int minX = boundaryLeftPoint.x + offset;
        int maxX = boundaryRightPoint.x - offset;
        int minY = boundaryLeftPoint.y + offset;
        int maxY = boundaryRightPoint.y - offset;

        // Generate random point in the lower-left portion of the boundary
        return new Vector2Int(
            Random.Range(minX, (int)(minX + (maxX - minX) * pointModifier)),
            Random.Range(minY, (int)(minY + (maxY - minY) * pointModifier)));
    }

    /// <summary>
    /// Generates a random top-right corner coordinate within a boundary with an offset and modifier
    /// Used to create variation in room placement within partition spaces
    /// </summary>
    /// <param name="boundaryLeftPoint">The minimum corner of the boundary area</param>
    /// <param name="boundaryRightPoint">The maximum corner of the boundary area</param>
    /// <param name="pointModifier">A percentage (0-1) to limit how far from the end the point starts</param>
    /// <param name="offset">Distance to inset from the boundary edges</param>
    /// <returns>A random point within the specified range</returns>
    public static Vector2Int GenerateTopRightCornerBetween(
        Vector2Int boundaryLeftPoint, Vector2Int boundaryRightPoint, float pointModifier, int offset)
    {
        int minX = boundaryLeftPoint.x + offset;
        int maxX = boundaryRightPoint.x - offset;
        int minY = boundaryLeftPoint.y + offset;
        int maxY = boundaryRightPoint.y - offset;

        // Generate random point in the upper-right portion of the boundary
        return new Vector2Int(
            Random.Range((int)(minX + (maxX - minX) * pointModifier), maxX),
            Random.Range((int)(minY + (maxY - minY) * pointModifier), maxY));
    }

    /// <summary>
    /// Calculates the middle point between two 2D integer coordinates
    /// Used to find centerpoints for corridor placement
    /// </summary>
    /// <param name="v1">First coordinate</param>
    /// <param name="v2">Second coordinate</param>
    /// <returns>The midpoint between the two coordinates</returns>
    public static Vector2Int CalculateMiddlePoint(Vector2Int v1, Vector2Int v2)
    {
        Vector2 sum = v1 + v2;
        Vector2 tempVector = sum / 2;
        return new Vector2Int((int)tempVector.x, (int)tempVector.y);
    }
}

/// <summary> Enum to describe the relative position of one structure compared to another </summary>
public enum RelativePosition
{
    /// <summary> Structure is above </summary>
    Up,
    
    /// <summary> Structure is below </summary>
    Down,
    
    /// <summary> Structure is to the left </summary>
    Left,
    
    /// <summary> Structure is to the right </summary>
    Right
}
