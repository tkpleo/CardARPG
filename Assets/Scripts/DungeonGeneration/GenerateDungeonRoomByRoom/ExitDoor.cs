using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ExitDoor : MonoBehaviour
{
    internal bool reachedExit = false;

    void Awake()
    {
        // Ensure the BoxCollider is set as a trigger
        BoxCollider collider = GetComponent<BoxCollider>();
        collider.isTrigger = true;
    }

    public Vector2Int GetDirectionVector()
    {
        // This method should return the direction vector based on the exit door's orientation
        // For example, if the door is facing north, return (0, 1)
        // This is a placeholder implementation and should be expanded based on your specific needs
        return Vector2Int.up; // Default to north for now
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player has entered the exit door
        if (other.CompareTag("Player") && !reachedExit)
        {
            var grandParentRoomCreator = GetComponentInParent<RoomCreator>();
            if(grandParentRoomCreator != null)
                grandParentRoomCreator.CreateNextRoom(grandParentRoomCreator.GetCurrentRoom());
            Debug.Log("Player has reached the exit door!");
            reachedExit = true;
            // Reset reachedExit after transition
            StartCoroutine(ResetReachedExit());
        }
    }

    private System.Collections.IEnumerator ResetReachedExit()
    {
        yield return new WaitForSeconds(1f);
        reachedExit = false;
    }
}
