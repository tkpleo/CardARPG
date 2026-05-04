using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using DeckBuilding;

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

    [Header("Enemy Spawner Settings")]
    [SerializeField] private GameObject enemySpawnPointPrefab;
    [SerializeField] private int maxSpawnersPerRoom = 3;
    [SerializeField] private float minSpawnerDistance = 2f;
    [SerializeField] private List<GameObject> spawners;
    private List<EnemyBehavior> spawnedEnemies;
    private Coroutine waitForEnemiesCoroutine;
    private const int roomBorderPadding = 2;

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

    public event Action OnExitDoorActivated;

    
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

    private Room startRoom;

    private void Awake()
    {
        currentLevelGameObjects = new List<GameObject>();
        gaps = new List<GameObject>();
        spawners = new List<GameObject>();
        spawnedEnemies = new List<EnemyBehavior>();
        currentLevel = new LevelData();
    }

    private void Start()
    {
        currentLevelGameObjects = new List<GameObject>();
        currentLevel = new LevelData();
        spawnedEnemies = new List<EnemyBehavior>();

        InitializeStartingRoom();
        LogAllSpawnerEnemies();
        StartEnemyClearCheck();
    }

    private void StartEnemyClearCheck()
    {
        if (waitForEnemiesCoroutine != null)
        {
            StopCoroutine(waitForEnemiesCoroutine);
        }
        waitForEnemiesCoroutine = StartCoroutine(WaitForEnemiesDefeated());
    }

    private void LogAllSpawnerEnemies()
    {
        foreach (var spawner in spawners)
        {
            EnemySpawnpoint spawnpointScript = spawner.GetComponent<EnemySpawnpoint>();
            if (spawnpointScript != null && spawnpointScript.spawnedEnemy != null)
            {
                spawnedEnemies.Add(spawnpointScript.spawnedEnemy);
            }
            else
            {
                Debug.Log($"Spawner at {spawner.transform.position} has no spawned enemy.");
            }
        }
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

        startRoom = startingRoom; // Ensure current room reference is set for spawner placement

        // Visualize the room and track its GameObject
        VisualizeRoom(startingRoom);

        DeckManager.ReshuffleDiscardIntoDraw();

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

        roomStructureCreator.CreateEntrance(room, currentLevelGameObjects, levelRootTransform);
        roomStructureCreator.CreateExit(room, currentLevelGameObjects, levelRootTransform, exitDoorPrefab);

        // Destroy all enemies from the previous room
        List<EnemyBehavior> enemiesToDestroy = new List<EnemyBehavior>(spawnedEnemies);

        if (enemiesToDestroy.Count == 0)
        {
            Debug.Log("No enemies to destroy from previous room.");
        }
        else
        {
            foreach (var enemy in enemiesToDestroy)
        {
            if (enemy != null && enemy.gameObject != null)
            {
                Destroy(enemy.gameObject);
            }
        }
        spawnedEnemies.Clear();
        }
        

        // Destroy all spawners from the previous room
        List<GameObject> spawnersToDestroy = new List<GameObject>(spawners);
        foreach (var spawner in spawnersToDestroy)
        {
            Destroy(spawner);
        }
        spawners.Clear();

        gaps.Clear();
        GapCreator.MakeRandomGapsInFloor(room.position, room.position + room.size, gapOffsetFromWalls, 
        room, currentLevelGameObjects, levelRootTransform, gapWidthMin, gapWidthMax, gapLengthMin, gapLengthMax, 
        minGapCount, maxGapCount, gapMaterial, gaps);
        
        PlaceSpawnersInRoom(room);

        DeckManager.StartCombat();
        
        // Track newly spawned enemies
        LogAllSpawnerEnemies();
        StartEnemyClearCheck();
        
        currentLevelGameObjects.Add(roomObject);
        
        Debug.Log($"Visualized room at {room.position} with size {room.size}, entrance: {room.enteranceDirection}, exit: {room.exitDirection}");
    }

    #endregion

    private void PlaceSpawnersInRoom(Room room)
    {
        if (enemySpawnPointPrefab == null)
        {
            Debug.LogError("Enemy spawn point prefab is not assigned in RoomCreator.");
            return;
        }

        for (int i = 0; i < maxSpawnersPerRoom; i++)
        {
            TryPlaceSpawner(room);
        }
    }

    private void TryPlaceSpawner(Room room)
    {
        Vector2Int roomStart = room.position;
        Vector2Int roomEnd = room.position + room.size;
        int minX = roomStart.x + roomBorderPadding;
        int maxX = roomEnd.x - roomBorderPadding;
        int minY = roomStart.y + roomBorderPadding;
        int maxY = roomEnd.y - roomBorderPadding;

        if (minX > maxX || minY > maxY)
        {
            Debug.LogWarning($"Room at {room.position} is too small to place spawners with padding {roomBorderPadding}.");
            return;
        }

        Vector2Int spawnPosition = new Vector2Int(Random.Range(minX, maxX + 1), Random.Range(minY, maxY + 1));
        const int maxAttempts = 100;
        int attempt = 0;

        while ((IsPositionOverlappingGap(spawnPosition) || IsPositionNearOtherSpawner(spawnPosition, minSpawnerDistance) || !IsPositionInsideRoom(spawnPosition, room))
               && attempt < maxAttempts)
        {
            spawnPosition = new Vector2Int(Random.Range(minX, maxX + 1), Random.Range(minY, maxY + 1));
            attempt++;
        }

        if (attempt >= maxAttempts)
        {
            Debug.LogWarning($"Could not find valid spawn position in room at {room.position} after {maxAttempts} attempts.");
            return;
        }

        GameObject spawnPoint = Instantiate(enemySpawnPointPrefab, new Vector3(spawnPosition.x, .5f, spawnPosition.y), Quaternion.identity);
        if (levelRootTransform != null)
            spawnPoint.transform.SetParent(levelRootTransform);
        currentLevelGameObjects.Add(spawnPoint);
        spawners.Add(spawnPoint);
        spawnPoint.name = $"EnemySpawner_{spawnPosition.x}_{spawnPosition.y}";
    }

    private bool IsPositionNearOtherSpawner(Vector2Int position, float minDistance)
    {
        foreach (var spawner in spawners)
        {
            Vector3 spawnerPos = spawner.transform.position;
            if (Vector2.Distance(new Vector2(position.x, position.y), new Vector2(spawnerPos.x, spawnerPos.z)) < minDistance)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsPositionInsideRoom(Vector2Int position, Room room)
    {
        return position.x >= room.position.x + roomBorderPadding && position.x <= room.position.x + room.size.x - roomBorderPadding &&
               position.y >= room.position.y + roomBorderPadding && position.y <= room.position.y + room.size.y - roomBorderPadding;
    }

    private bool IsPositionOverlappingGap(Vector2Int position)
    {
        const float spawnPadding = 0.5f;
        foreach (var gap in gaps)
        {
            Vector3 gapPos = gap.transform.position;
            Vector3 gapScale = gap.transform.localScale;
            float halfWidth = gapScale.x / 2f + spawnPadding;
            float halfLength = gapScale.z / 2f + spawnPadding;
            if (position.x >= gapPos.x - halfWidth && position.x <= gapPos.x + halfWidth &&
                position.y >= gapPos.z - halfLength && position.y <= gapPos.z + halfLength)
            {
                return true;
            }
        }
        return false;
    }

    public void CreateNextRoom(Room currentRoom)
    {
        StartCoroutine(DoRoomTransition(currentRoom));
    }

    private IEnumerator DoRoomTransition(Room currentRoom)
    {
        var loadingScreen = canvas.GetComponent<LoadingScreen>();
        if (loadingScreen != null)
            StartCoroutine(loadingScreen.LoadingScreenCoroutine());
            
    
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

    private IEnumerator WaitForEnemiesDefeated()
    {
        while (spawnedEnemies.Exists(enemy => enemy != null && enemy.currentHealth > 0))
        {
            yield return null; // Wait until the next frame and check again
        }
        Debug.Log("All enemies defeated! Activating exit door.");
        OnExitDoorActivated?.Invoke();
    }

}




