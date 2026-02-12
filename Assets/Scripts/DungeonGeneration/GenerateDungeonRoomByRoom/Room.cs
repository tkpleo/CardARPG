using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a single room in the dungeon
/// Stores position, size, and exit points to other rooms
/// </summary>
public class Room
{
    /// <summary> The bottom-left corner position of the room </summary>
    public Vector2Int position;
    
    /// <summary> The width and height of the room </summary>
    public Vector2Int size;
    
    /// <summary> The direction of the exit from this room </summary>
    public directions exitDirection;
    public directions enteranceDirection; // Optional: Direction from which the player enters the room
    public int roomID; // Optional: Unique identifier for the room, useful for debugging or tracking

    /// <summary>
    /// Constructor to create a room with specified position and size
    /// </summary>
    /// <param name="position">Bottom-left corner of the room</param>
    /// <param name="size">Width and height of the room</param>
    public Room(Vector2Int position, Vector2Int size, int roomID = -1)
    {
        this.position = position;
        this.size = size;
        this.enteranceDirection = directions.North; // Default entrance direction
        this.exitDirection = GetRandomExitDirection(this.enteranceDirection); // Random exit direction different from entrance
        this.roomID = roomID; // Default ID, can be set later
    }

    /// <summary>
    /// Gets the center position of the room
    /// </summary>
    /// <returns>The center point of the room</returns>
    public Vector2 GetCenter()
    {
        return new Vector2(position.x + size.x / 2f, position.y + size.y / 2f);
    }

    public directions GetRandomExitDirection(directions enteranceDirection)
    {
        List<directions> possibleExitDirections = new List<directions> { directions.North, directions.South, directions.East, directions.West };
        possibleExitDirections.Remove(enteranceDirection);
        return possibleExitDirections[Random.Range(0, possibleExitDirections.Count)];
    }

    public Vector2Int GetExitPosition()
    {
        switch (exitDirection)
        {
            case directions.North:
                return new Vector2Int(position.x + size.x / 2, position.y + size.y);
            case directions.South:
                return new Vector2Int(position.x + size.x / 2, position.y);
            case directions.East:
                return new Vector2Int(position.x + size.x, position.y + size.y / 2);
            case directions.West:
                return new Vector2Int(position.x, position.y + size.y / 2);
            default:
                return position; // Default to bottom-left corner if something goes wrong
        }
    }
}

public enum directions
{
    North,
    South,
    East,
    West
}
