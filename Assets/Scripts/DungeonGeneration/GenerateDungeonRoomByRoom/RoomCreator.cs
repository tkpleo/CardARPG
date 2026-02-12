using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// MonoBehaviour that manages room creation and level progression for room-by-room dungeon generation
/// Handles creating individual rooms, level transitions with loading screens, and proper cleanup
/// </summary>
public class RoomCreator : MonoBehaviour
{
    /// <summary> Minimum width for generated rooms </summary>
    [SerializeField] private int roomWidthMin;
    
    /// <summary> Minimum height for generated rooms </summary>
    [SerializeField] private int roomLengthMin;
    
    /// <summary> Maximum width for generated rooms </summary>
    [SerializeField] private int roomWidthMax;
    
    /// <summary> Maximum height for generated rooms </summary>
    [SerializeField] private int roomLengthMax;
    
    /// <summary> Parent transform to organize level GameObjects </summary>
    [SerializeField] private Transform levelRootTransform;

    [SerializeField] private Material floorMaterial;
    
    /// <summary> Current level number/index </summary>
    private int currentLevelNumber = 1;
    
    /// <summary> The current level's room data </summary>
    private LevelData currentLevel;
    
    /// <summary> Builder for creating individual rooms </summary>
    private RoomBuilder roomBuilder;
    
    /// <summary> All GameObjects created for the current level (for cleanup) </summary>
    private List<GameObject> currentLevelGameObjects;

    /// <summary>
    /// Called when the script initializes
    /// Sets up the initial level
    /// </summary>
    private void Start()
    {
        currentLevelGameObjects = new List<GameObject>();
        currentLevel = new LevelData();
        InitializeStartingRoom();
    }

    private void InitializeStartingRoom()
    {
        InitializeLevel();
        
    }

    /// <summary>
    /// Initializes the current level by creating the starting room
    /// </summary>
    public void InitializeLevel()
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
        if (levelRootTransform != null)
        {
            DestroyAllChildren(levelRootTransform.gameObject);
        }

        // Initialize if needed (for editor button use)
        if (currentLevel == null)
        {
            currentLevelGameObjects = new List<GameObject>();
            currentLevel = new LevelData();
        }

        Debug.Log($"Initializing Level {currentLevelNumber}");
    
        // Create the first room at origin
        int randomWidth = Random.Range(roomWidthMin, roomWidthMax + 1);
        int randomLength = Random.Range(roomLengthMin, roomLengthMax + 1);
        roomBuilder = new RoomBuilder(randomWidth, randomLength);
        
        Room startingRoom = roomBuilder.BuildRoom(Vector2Int.zero);
        currentLevel.AddRoom(startingRoom);
        currentLevel.SetStartingRoom(startingRoom);

        // Visualize the room and track its GameObject
        VisualizeRoom(startingRoom);

        startingRoom.roomID = currentLevelNumber; // Assign room ID based on level number for tracking
    }

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

        // Wall positions will be calculated directly in AttachWalls()
    }

    private GameObject CreatePrimitiveWall(Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = position;
        wall.transform.localScale = scale;
        wall.transform.parent = levelRootTransform; // Parent to the level root for organization
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

    /// <summary>
    /// Visualizes a room in the scene by creating a GameObject with floor and walls
    /// </summary>
    /// <param name="room">The room to visualize</param>
    private void VisualizeRoom(Room room)
    {
        // Create parent GameObject for this room
        GameObject roomObject = new GameObject($"Room_Level{currentLevelNumber}_{room.position}");
        
        if (levelRootTransform != null)
            roomObject.transform.SetParent(levelRootTransform);
        
        roomObject.transform.position = new Vector3(room.position.x, 0, room.position.y);
        
        CreateFloor(room.position, room.position + room.size);
        AttachWalls();
        
        CreateEntrance(room);
        CreateExit(room);
        
        currentLevelGameObjects.Add(roomObject);
        
        Debug.Log($"Visualized room at {room.position} with size {room.size}");
    }

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
}




