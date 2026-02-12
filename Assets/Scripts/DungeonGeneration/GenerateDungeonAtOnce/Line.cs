using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// Represents a division line used in Binary Space Partitioning.
/// Stores the orientation (horizontal or vertical) and coordinates of the line.
/// </summary>
public class Line : MonoBehaviour
{
    /// <summary> The orientation of the line (Horizontal or Vertical) </summary>
    Orientation orientation;
    
    /// <summary> The coordinate position of the line (x for vertical, y for horizontal) </summary>
    Vector2Int coordinates;

    /// <summary> Property to access and modify the line's orientation </summary>
    public Orientation Orientation { get => orientation; set => orientation = value; }
    
    /// <summary> Property to access and modify the line's coordinates </summary>
    public Vector2Int Coordinates { get => coordinates; set => coordinates = value; }

    /// <summary>
    /// Constructor to initialize a line with a specific orientation and coordinate position
    /// </summary>
    /// <param name="orientation">Horizontal or Vertical orientation</param>
    /// <param name="coordinates">The position of the line on the grid</param>
    public Line(Orientation orientation, Vector2Int coordinates)
    {
        this.orientation = orientation;
        this.coordinates = coordinates;
    }
}

/// <summary> Enum to define the orientation of a line used in space partitioning </summary>

public enum Orientation
{
    Horizontal = 0,
    Vertical = 1
}