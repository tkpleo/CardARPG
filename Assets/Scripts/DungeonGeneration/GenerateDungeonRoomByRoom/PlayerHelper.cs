using UnityEngine;

public class PlayerHelper : MonoBehaviour
{
    public void RepositionPlayerToEntrance(Room room, GameObject player, LevelData currentLevel)
    {
        room = currentLevel.currentRoom; // Ensure we're using the current room reference
        Vector3 entrancePosition = room.enteranceDirection switch
        {
            directions.North => new Vector3(room.position.x + room.size.x / 2, 0, room.position.y + room.size.y - 0.5f),
            directions.South => new Vector3(room.position.x + room.size.x / 2, 0, room.position.y + 0.5f),
            directions.East => new Vector3(room.position.x + room.size.x - 0.5f, 0, room.position.y + room.size.y / 2),
            directions.West => new Vector3(room.position.x + 0.5f, 0, room.position.y + room.size.y / 2),
            _ => new Vector3(room.position.x + room.size.x / 2, 0, room.position.y + room.size.y / 2) // Default to center if something goes wrong
        };
        Debug.Log($"Repositioning player to entrance at {entrancePosition} for room {room.roomID}");
        if (player != null)
        {
            Debug.Log($"Player position before reposition: {player.transform.position}");
            // Move the player further away from the entrance to avoid immediate retrigger
            Vector3 offset = room.enteranceDirection switch
            {
                directions.North => new Vector3(0, 0, -2f),
                directions.South => new Vector3(0, 0, 2f),
                directions.East => new Vector3(-2f, 0, 0),
                directions.West => new Vector3(2f, 0, 0),
                _ => Vector3.zero
            };
            var characterController = player.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.enabled = false;
                player.transform.position = entrancePosition + offset;
                characterController.enabled = true;
            }
            else
            {
                player.transform.position = entrancePosition + offset;
            }
            Debug.Log($"Player position after reposition: {player.transform.position}");
        }
        else
        {
            Debug.LogWarning("Player GameObject with tag 'Player' not found for repositioning.");
        }
    }
}
