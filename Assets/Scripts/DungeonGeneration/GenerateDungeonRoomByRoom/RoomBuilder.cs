using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Builds individual rooms for room-by-room dungeon generation
/// Handles room size calculation and positioning
/// </summary>
public class RoomBuilder
{
    public RoomBuilder() {}

    public Room BuildNextRoom(Room currentRoom, int roomWidth, int roomLength)
    {
        directions newEntranceDir = Room.GetOppositeDirection(currentRoom.exitDirection);
        Vector2Int prevExitPos = currentRoom.GetExitPosition();
        Vector2Int newRoomPos = prevExitPos;
        switch (newEntranceDir)
        {
            case directions.North:
                newRoomPos -= new Vector2Int(roomWidth / 2, roomLength);
                break;
            case directions.South:
                newRoomPos -= new Vector2Int(roomWidth / 2, 0);
                break;
            case directions.East:
                newRoomPos -= new Vector2Int(roomWidth, roomLength / 2);
                break;
            case directions.West:
                newRoomPos -= new Vector2Int(0, roomLength / 2);
                break;
        }
        Room nextRoom = BuildRoom(newRoomPos, roomWidth, roomLength);
        nextRoom.enteranceDirection = newEntranceDir;
        nextRoom.exitDirection = nextRoom.GetRandomExitDirection(
            nextRoom.enteranceDirection,
            currentRoom.exitDirection
        );
        return nextRoom;
    }

    /// <summary>
    /// Creates a single room at the specified position
    /// </summary>
    /// <param name="position">The bottom-left corner position of the room</param>
    /// <returns>A new Room object with the specified dimensions and position</returns>
    public Room BuildRoom(Vector2Int position, int roomWidth, int roomLength)
    {
        return new Room(position, new Vector2Int(roomWidth, roomLength));
    }
}
