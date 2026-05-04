using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ExitDoor : MonoBehaviour
{
    [SerializeField] private bool exitActivated = false;
    internal bool reachedExit = false;
    private RoomCreator roomCreator;

    void Awake()
    {
        // Ensure the BoxCollider is set as a trigger
        BoxCollider collider = GetComponent<BoxCollider>();
        collider.isTrigger = true;
    }

    private void OnEnable()
    {
        roomCreator = GetComponentInParent<RoomCreator>();
        if (roomCreator == null)
        {
            roomCreator = FindObjectOfType<RoomCreator>();
        }

        if (roomCreator != null)
        {
            roomCreator.OnExitDoorActivated += ActivateExitDoor;
        }
        else
        {
            Debug.LogWarning("ExitDoor could not find RoomCreator to subscribe to activation events.");
        }
    }

    private void OnDisable()
    {
        if (roomCreator != null)
        {
            roomCreator.OnExitDoorActivated -= ActivateExitDoor;
            roomCreator = null;
        }
    }

    private void Start()
    {
        // Initialize exit door state without disabling the GameObject.
        exitActivated = false;
        reachedExit = false;

        var renderer = GetComponent<Renderer>();
        if (renderer != null)
            renderer.enabled = false;

        var collider = GetComponent<Collider>();
        if (collider != null)
            collider.enabled = false;
    }

    private void ActivateExitDoor()
    {
        exitActivated = true;

        var renderer = GetComponent<Renderer>();
        if (renderer != null)
            renderer.enabled = true;

        var collider = GetComponent<Collider>();
        if (collider != null)
            collider.enabled = true;

        Debug.Log("Exit door activated! Player can now exit the room.");
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
        if (!exitActivated)
            return;

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
