using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// MonoBehaviour that manages room creation and level progression for room-by-room dungeon generation
/// Handles creating individual rooms, level transitions with loading screens, and proper cleanup
/// </summary>
public class RoomCreator : MonoBehaviour
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
    
    [Header("Root Level Transform")]
    /// <summary> Parent transform to organize level GameObjects </summary>
    [SerializeField] private Transform levelRootTransform;

    
    private int roomLength;
    private int roomWidth;
    
    /// <summary> Current level number/index </summary>
    private int currentLevelNumber = 1;
    
    /// <summary> The current level's room data </summary>
    private LevelData currentLevel;
    
    /// <summary> Builder for creating individual rooms </summary>
    private RoomBuilder roomBuilder;
    
    /// <summary> All GameObjects created for the current level (for cleanup) </summary>
    private List<GameObject> currentLevelGameObjects;
    private List<GameObject> gaps;

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
    }

    /// <summary>
    /// Initializes the current level by creating the starting room
    /// </summary>
    public void InitializeLevel()
    {
        CleanUpPreviousLevel();
        
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
        roomBuilder = new RoomBuilder(roomWidth, roomLength);
        
        Room startingRoom = roomBuilder.BuildRoom(Vector2Int.zero);
        currentLevel.AddRoom(startingRoom);
        currentLevel.SetStartingRoom(startingRoom);

        // Visualize the room and track its GameObject
        VisualizeRoom(startingRoom);

        startingRoom.roomID = currentLevelNumber; // Assign room ID based on level number for tracking
    }

    private void VisualizeRoom(Room room)
    {
        // Create parent GameObject for this room
        GameObject roomObject = new GameObject($"Room_Level{currentLevelNumber}_{room.position}");
        
        if (levelRootTransform != null)
            roomObject.transform.SetParent(levelRootTransform);
        
        roomObject.transform.position = new Vector3(room.position.x, 0, room.position.y);
        
        CreateFloor(room.position, room.position + room.size);
        AttachWalls();
        MakeRandomGapsInFloor(room.position, room.position + room.size, gapOffsetFromWalls, room);
        
        CreateEntrance(room);
        CreateExit(room);
        
        currentLevelGameObjects.Add(roomObject);
        
        Debug.Log($"Visualized room at {room.position} with size {room.size}");
    }

    #endregion

    #region Floor Creation

    private void CreateFloor(Vector2Int bottomLeftCorner, Vector2Int topRightCorner)
    {
        Vector3 bottomLeftV = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightV = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftV = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightV = new Vector3(topRightCorner.x, 0, topRightCorner.y);

        Vector3[] vertices = new Vector3[] {topLeftV, topRightV, bottomLeftV, bottomRightV};

        Vector2[] uvs = new Vector2[vertices.Length];

        for(int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        int[] triangles = new int[]
        {
            0, 1, 2,
            2, 1, 3
        };

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        
        GameObject dungeonFloor = new GameObject("DungeonFloor"+bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));

        dungeonFloor.transform.parent = levelRootTransform;

        dungeonFloor.transform.position = Vector3.zero;
        dungeonFloor.transform.localScale = Vector3.one;

        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = floorMaterial;
        currentLevelGameObjects.Add(dungeonFloor);

    }

    private void MakeRandomGapsInFloor(Vector2Int bottomLeftCorner, Vector2Int topRightCorner, float offsetFromWalls, Room room)
    {
        gaps = new List<GameObject>();

        var gapCount = Random.Range(minGapCount, maxGapCount + 1);

        for (int i = 0; i < gapCount; i++)
        {
            Vector2Int exitPos = room.GetExitPosition();

            // Generate gap size first
            Vector3 gapScale = new Vector3(Random.Range(gapWidthMin, gapWidthMax + 1), 0.02f, Random.Range(gapLengthMin, gapLengthMax + 1));
            
            // Find a valid position for this gap
            Vector2 validPosition = FindValidGapPosition(bottomLeftCorner, topRightCorner, offsetFromWalls, exitPos, gapScale, gaps);
            
            Vector3 gapPosition = new Vector3(validPosition.x, 0.01f, validPosition.y); // Slightly above floor to avoid z-fighting
            GameObject gap = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gap.transform.position = gapPosition;
            gap.transform.localScale = gapScale;
            gap.GetComponent<Renderer>().material = gapMaterial; // Use the assigned gap material for visibility
            BoxCollider boxCollider = gap.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(1, 50, 1); // Ensure collider matches the visual size of the gap
            boxCollider.center = new Vector3(0, 25, 0);
            gap.name = "Gap_" + i;
            currentLevelGameObjects.Add(gap);
            gap.transform.parent = levelRootTransform; // Parent to level root for organization
            gaps.Add(gap);
        }
    }

    /// <summary>
    /// Finds a valid position for a gap that doesn't overlap with the exit or other gaps
    /// </summary>
    private Vector2 FindValidGapPosition(Vector2Int bottomLeftCorner, Vector2Int topRightCorner, float offsetFromWalls, 
        Vector2Int exitPos, Vector3 gapScale, List<GameObject> existingGaps)
    {
        int maxAttempts = 50; // Prevent infinite loop
        
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            float randomX = Random.Range(bottomLeftCorner.x + offsetFromWalls, topRightCorner.x - offsetFromWalls);
            float randomY = Random.Range(bottomLeftCorner.y + offsetFromWalls, topRightCorner.y - offsetFromWalls);
            
            // Check if this position is valid
            if (IsValidGapPosition(randomX, randomY, gapScale, exitPos, existingGaps))
            {
                return new Vector2(randomX, randomY);
            }
        }
        
        // If we couldn't find a valid position after max attempts, return a random one (fallback)
        Debug.LogWarning("Could not find non-overlapping position for gap after " + maxAttempts + " attempts");
        return new Vector2(
            Random.Range(bottomLeftCorner.x + offsetFromWalls, topRightCorner.x - offsetFromWalls),
            Random.Range(bottomLeftCorner.y + offsetFromWalls, topRightCorner.y - offsetFromWalls)
        );
    }
    
    /// <summary>
    /// Checks if a gap position is valid (doesn't overlap with exit or other gaps)
    /// </summary>
    private bool IsValidGapPosition(float posX, float posY, Vector3 gapScale, Vector2Int exitPos, List<GameObject> existingGaps)
    {
        // Add some padding around the exit position
        float exitPadding = 2f;
        
        // Check if overlaps with exit position
        if (Mathf.Abs(posX - exitPos.x) < (gapScale.x / 2 + exitPadding) &&
            Mathf.Abs(posY - exitPos.y) < (gapScale.z / 2 + exitPadding))
        {
            return false;
        }
        
        // Check if overlaps with any existing gaps
        foreach (GameObject existingGap in existingGaps)
        {
            Vector3 otherPos = existingGap.transform.position;
            Vector3 otherScale = existingGap.transform.localScale;
            
            // AABB collision check with small padding to prevent gaps from being too close
            float padding = gapOffsetFromEachOther;
            if (Mathf.Abs(posX - otherPos.x) < (gapScale.x + otherScale.x) / 2 + padding &&
                Mathf.Abs(posY - otherPos.z) < (gapScale.z + otherScale.z) / 2 + padding)
            {
                return false;
            }
        }
        
        return true;
    }

    #endregion

    #region Wall Creation
    private GameObject CreatePrimitiveWall(Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = position;
        wall.transform.localScale = scale;
        wall.transform.parent = levelRootTransform; // Parent to the level root for organization
        wall.GetComponent<Renderer>().material = wallMaterial; // Use the assigned wall material
        return wall;
    }

    private void AttachWalls()
    {
        Room currentRoom = currentLevel.currentRoom;
        Vector2Int roomSize = currentRoom.size;
        Vector2Int roomPos = currentRoom.position;
        
        float roomWidth = roomSize.x;
        float roomDepth = roomSize.y;
        float wallThickness = 0.2f;
        float wallHeight = 3f;
        
        // Wall positions: center of each wall
        // Bottom wall (negative Z)
        Vector3 bottomWallPos = new Vector3(roomPos.x + roomWidth / 2, 0, roomPos.y - wallThickness / 2);
        Vector3 bottomWallScale = new Vector3(roomWidth, wallHeight, wallThickness);
        GameObject bottomWall = CreatePrimitiveWall(bottomWallPos, bottomWallScale);
        bottomWall.name = "BottomWall";
        currentLevelGameObjects.Add(bottomWall);
        
        // Top wall (positive Z)
        Vector3 topWallPos = new Vector3(roomPos.x + roomWidth / 2, 0, roomPos.y + roomDepth + wallThickness / 2);
        Vector3 topWallScale = new Vector3(roomWidth, wallHeight, wallThickness);
        GameObject topWall = CreatePrimitiveWall(topWallPos, topWallScale);
        topWall.name = "TopWall";
        currentLevelGameObjects.Add(topWall);
        
        // Left wall (negative X)
        Vector3 leftWallPos = new Vector3(roomPos.x - wallThickness / 2, 0, roomPos.y + roomDepth / 2);
        Vector3 leftWallScale = new Vector3(wallThickness, wallHeight, roomDepth);
        GameObject leftWall = CreatePrimitiveWall(leftWallPos, leftWallScale);
        leftWall.name = "LeftWall";
        currentLevelGameObjects.Add(leftWall);
        
        // Right wall (positive X)
        Vector3 rightWallPos = new Vector3(roomPos.x + roomWidth + wallThickness / 2, 0, roomPos.y + roomDepth / 2);
        Vector3 rightWallScale = new Vector3(wallThickness, wallHeight, roomDepth);
        GameObject rightWall = CreatePrimitiveWall(rightWallPos, rightWallScale);
        rightWall.name = "RightWall";
        currentLevelGameObjects.Add(rightWall);
    }
    #endregion
    /// <summary>
    /// Visualizes a room in the scene by creating a GameObject with floor and walls
    /// </summary>
    /// <param name="room">The room to visualize</param>
    

    #region Entrance and Exit Creation
    private void CreateEntrance(Room room)
    {
        Vector3 entrancePosition = GetExitPosition(room, room.enteranceDirection);
        GameObject entranceObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        entranceObject.transform.position = entrancePosition;
        entranceObject.transform.localScale = new Vector3(1, 2, 1); // Example size for the entrance
        entranceObject.name = $"Entrance_Direction_{room.enteranceDirection}";
        entranceObject.transform.parent = levelRootTransform;
        currentLevelGameObjects.Add(entranceObject);
    }
    private void CreateExit(Room room)
    { 
        Vector3 exitPosition = GetExitPosition(room, room.exitDirection);
        GameObject exitObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        exitObject.transform.position = exitPosition;
        exitObject.transform.localScale = new Vector3(1, 2, 1); // Example size for the exit
        exitObject.name = $"Exit_Direction_{room.exitDirection}";
        exitObject.transform.parent = levelRootTransform;
        currentLevelGameObjects.Add(exitObject);
    }

    private Vector3 GetExitPosition(Room room, directions exitDirection)
    {
        Vector2Int roomPos = room.position;
        Vector2Int roomSize = room.size;
        
        switch (exitDirection)
        {
            case directions.North:
                return new Vector3(roomPos.x + roomSize.x / 2, 0, roomPos.y + roomSize.y);
            case directions.South:
                return new Vector3(roomPos.x + roomSize.x / 2, 0, roomPos.y);
            case directions.East:
                return new Vector3(roomPos.x + roomSize.x, 0, roomPos.y + roomSize.y / 2);
            case directions.West:
                return new Vector3(roomPos.x, 0, roomPos.y + roomSize.y / 2);
            default:
                return Vector3.zero; // Default case, should not happen
        }
    }
    #endregion

    #region Helper Methods
    private void DestroyAllChildren(GameObject parent)
    {
        if (parent == null) return;

        // Collect children first to avoid modification during iteration
        List<Transform> children = new List<Transform>();
        foreach(Transform child in parent.transform)
        {
            children.Add(child);
        }

        // Destroy all collected children
        foreach(Transform child in children)
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                DestroyImmediate(child.gameObject);
            }
            else
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void CleanUpPreviousLevel()
    {
        // Clean up existing GameObjects from previous levels
        foreach(GameObject obj in currentLevelGameObjects)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }
        currentLevelGameObjects.Clear();

        // Also destroy children under levelRootTransform for safety
        if (levelRootTransform != null && levelRootTransform.childCount > 0)
        {
            DestroyAllChildren(levelRootTransform.gameObject);
        }
    }
    #endregion
}




