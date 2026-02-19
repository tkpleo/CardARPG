using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// MonoBehaviour that manages room creation and level progression for room-by-room dungeon generation
/// Handles creating individual rooms, level transitions with loading screens, and proper cleanup
/// </summary>
public class RoomCreator : MonoBehaviour
    // Prevents rapid room creation during transitions
    {
    
    [Header("Room Settings")]
    /// <summary> Minimum width for generated rooms </summary>
    [SerializeField] private int roomWidthMin;
    
    /// <summary> Minimum height for generated rooms </summary>
    [SerializeField] private int roomLengthMin;
    
    /// <summary> Maximum width for generated rooms </summary>
    [SerializeField] private int roomWidthMax;
    
    /// <summary> Maximum height for generated rooms </summary>
    [SerializeField] private int roomLengthMax;
    [SerializeField] private Material floorMaterial;
    [SerializeField] private Material wallMaterial;
    [SerializeField] private GameObject exitDoorPrefab;

    [Header("Gap Settings")]
    [SerializeField] private int gapWidthMin;
    [SerializeField] private int gapWidthMax;
    [SerializeField] private int gapLengthMin;
    [SerializeField] private int gapLengthMax; 
    [SerializeField] private int minGapCount;
    [SerializeField] private int maxGapCount;
    [SerializeField] private float gapOffsetFromWalls;
    [SerializeField] private float gapOffsetFromEachOther;
    [SerializeField] private Material gapMaterial;
    
    [SerializeField] private GameObject player;

    [Header("Root Level Transform")]
    /// <summary> Parent transform to organize level GameObjects </summary>
    [SerializeField] private Transform levelRootTransform;

    [Header("Loading Screen Settings")]
    [SerializeField] private GameObject canvas;

    
    private int roomLength;
    private int roomWidth;
    
    /// <summary> Current level number/index </summary>
    private int currentLevelNumber = 1;
    
    /// <summary> The current level's room data </summary>
    private LevelData currentLevel;
    
    /// <summary> Builder for creating individual rooms </summary>
    private RoomBuilder roomBuilder;

    private CreateRoomStructure roomStructureCreator;

    private GapCreator gapCreator;

    private PlayerHelper playerHelper;
    
    /// <summary> All GameObjects created for the current level (for cleanup) </summary>
    private List<GameObject> currentLevelGameObjects;
    private List<GameObject> gaps;

    private void Awake()
    {
        currentLevelGameObjects = new List<GameObject>();
        currentLevel = new LevelData();
        // roomStructureCreator = new CreateRoomStructure(); // REMOVE
        // playerHelper = new PlayerHelper(); // REMOVE
    }

    private void Start()
    {
        currentLevelGameObjects = new List<GameObject>();
        currentLevel = new LevelData();

        InitializeStartingRoom();
    }

    #region Level Initialization and Progression 

    private void InitializeStartingRoom()
    {
        InitializeLevel();
        
        playerHelper.RepositionPlayerToEntrance(currentLevel.currentRoom, player, currentLevel);
    }

    /// <summary>
    /// Initializes the current level by creating the starting room
    /// </summary>
    public void InitializeLevel()
    {
        // LevelGenerationHelper levelHelper = new LevelGenerationHelper(); // REMOVE
        // levelHelper.CleanUpPreviousLevel(currentLevelGameObjects, levelRootTransform);
        LevelGenerationHelper.CleanUpPreviousLevel(currentLevelGameObjects, levelRootTransform);
        
        // Initialize if needed (for editor button use)
        if (currentLevel == null)
        {
            currentLevelGameObjects = new List<GameObject>();
            currentLevel = new LevelData();
        }

        Debug.Log($"Initializing Level {currentLevelNumber}");
    
        // Create the first room at origin
        roomWidth = Random.Range(roomWidthMin, roomWidthMax + 1);
        roomLength = Random.Range(roomLengthMin, roomLengthMax + 1);
        roomBuilder = new RoomBuilder();
        Room startingRoom = roomBuilder.BuildRoom(Vector2Int.zero, roomWidth, roomLength);
        Debug.Log($"[Init] Starting room size: {startingRoom.size}, entrance: {startingRoom.enteranceDirection}, exit: {startingRoom.exitDirection}");
        currentLevel.AddRoom(startingRoom);
        currentLevel.SetStartingRoom(startingRoom);

        // Visualize the room and track its GameObject
        VisualizeRoom(startingRoom);

        startingRoom.roomID = currentLevelNumber; // Assign room ID based on level number for tracking
        
        if(playerHelper == null)
        {
            Debug.LogError("playerHelper is null in InitializeLevel. Initializing...");
            playerHelper = new PlayerHelper();
        }
        playerHelper.RepositionPlayerToEntrance(startingRoom, player, currentLevel);
    }

    private void VisualizeRoom(Room room)
    {
        // Create parent GameObject for this room
        GameObject roomObject = new GameObject($"Room_Level{currentLevelNumber}_{room.position}");
        if (levelRootTransform != null)
            roomObject.transform.SetParent(levelRootTransform);
        roomObject.transform.position = new Vector3(room.position.x, 0, room.position.y);
        if (roomStructureCreator == null)
        {
            Debug.LogError("roomStructureCreator is null in VisualizeRoom. Initializing...");
            roomStructureCreator = new CreateRoomStructure();
        }
        roomStructureCreator.CreateFloor(room.position, room.position + room.size, currentLevelGameObjects, levelRootTransform, floorMaterial);
        roomStructureCreator.AttachWalls(room, currentLevelGameObjects, levelRootTransform, wallMaterial);

        List<GameObject> gaps = new List<GameObject>();
        GapCreator.MakeRandomGapsInFloor(room.position, room.position + room.size, gapOffsetFromWalls, 
        room, currentLevelGameObjects, levelRootTransform, gapWidthMin, gapWidthMax, gapLengthMin, gapLengthMax, 
        minGapCount, maxGapCount, gapMaterial, gaps);
        
        roomStructureCreator.CreateEntrance(room, currentLevelGameObjects, levelRootTransform);
        roomStructureCreator.CreateExit(room, currentLevelGameObjects, levelRootTransform, exitDoorPrefab);
        
        currentLevelGameObjects.Add(roomObject);
        
        Debug.Log($"Visualized room at {room.position} with size {room.size}, entrance: {room.enteranceDirection}, exit: {room.exitDirection}");
    }

    #endregion

    public void CreateNextRoom(Room currentRoom)
    {
        var loadingScreen = canvas.GetComponent<LoadingScreen>();
        if (loadingScreen != null)
            StartCoroutine(loadingScreen.LoadingScreenCoroutine());
        StartCoroutine(DoRoomTransition(currentRoom));
    }

    private IEnumerator DoRoomTransition(Room currentRoom)
    {
        Debug.Log("[Transition] Entered DoRoomTransition. Building next room immediately.");
        // Always randomize room size and create a new RoomBuilder for each new room
        roomWidth = Random.Range(roomWidthMin, roomWidthMax + 1);
        roomLength = Random.Range(roomLengthMin, roomLengthMax + 1);
        roomBuilder = new RoomBuilder();
        Room nextRoom = roomBuilder.BuildNextRoom(currentRoom, roomWidth, roomLength);
        Debug.Log($"[Transition] Next room size: {nextRoom.size}, entrance: {nextRoom.enteranceDirection}, exit: {nextRoom.exitDirection}");
        currentLevel.AddRoom(nextRoom);
        currentLevel.currentRoom = nextRoom;
        LevelGenerationHelper.CleanUpPreviousLevel(currentLevelGameObjects, levelRootTransform);
        // Visualize and reposition for the new room
        VisualizeRoom(currentLevel.currentRoom);

        if(playerHelper == null)
        {
            Debug.LogError("playerHelper is null in DoRoomTransition. Initializing...");
            playerHelper = new PlayerHelper();
        }
        playerHelper.RepositionPlayerToEntrance(currentLevel.currentRoom, player, currentLevel);
        // Wait a short moment to ensure player is moved before allowing another transition
        yield return new WaitForSeconds(0.5f);
    }
    
    public Room GetCurrentRoom()
    {
        return currentLevel.currentRoom;
    }
}




