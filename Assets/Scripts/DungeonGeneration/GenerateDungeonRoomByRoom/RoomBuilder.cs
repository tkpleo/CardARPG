using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Builds individual rooms for room-by-room dungeon generation
/// Handles room size calculation and positioning
/// </summary>
public class RoomBuilder
{
    /// <summary> The width of the room to build </summary>
    private int roomWidth;
    
    /// <summary> The length/height of the room to build </summary>
    private int roomLength;

    /// <summary>
    /// Constructor to initialize the room builder with room dimensions
    /// </summary>
    /// <param name="roomWidth">Width of the room</param>
    /// <param name="roomLength">Height of the room</param>
    public RoomBuilder(int roomWidth, int roomLength)
    {
        this.roomWidth = roomWidth;
        this.roomLength = roomLength;
    }

    /// <summary>
    /// Creates a single room at the specified position
    /// </summary>
    /// <param name="position">The bottom-left corner position of the room</param>
    /// <returns>A new Room object with the specified dimensions and position</returns>
    public Room BuildRoom(Vector2Int position)
    {
        return new Room(position, new Vector2Int(roomWidth, roomLength));
    }
}
