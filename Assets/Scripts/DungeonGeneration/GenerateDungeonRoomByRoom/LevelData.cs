using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages all room data for the current level/dungeon
/// Tracks the current room and all available rooms in the level
/// </summary>
public class LevelData
{
    /// <summary> List of all rooms in the current level </summary>
    public List<Room> rooms { get; private set; }
    
    /// <summary> The room the player is currently in </summary>
    public Room currentRoom { get; set; }

    /// <summary>
    /// Constructor to initialize level data
    /// </summary>
    public LevelData()
    {
        this.rooms = new List<Room>();
        this.currentRoom = null;
    }

    /// <summary>
    /// Adds a room to the level
    /// </summary>
    /// <param name="room">The room to add</param>
    public void AddRoom(Room room)
    {
        rooms.Add(room);
    }

    /// <summary>
    /// Sets the starting room (where the player begins)
    /// </summary>
    /// <param name="room">The starting room</param>
    public void SetStartingRoom(Room room)
    {
        currentRoom = room;
    }
}
