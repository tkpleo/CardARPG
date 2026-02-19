using System.Collections.Generic;
using UnityEngine;

public class CreateRoomStructure : MonoBehaviour
{
    public void CreateFloor(Vector2Int bottomLeftCorner, Vector2Int topRightCorner, 
    List<GameObject> currentLevelGameObjects, Transform levelRootTransform, Material floorMaterial)
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
        dungeonFloor.AddComponent<MeshCollider>().sharedMesh = mesh; // Add collider for player interaction
        currentLevelGameObjects.Add(dungeonFloor);

    }

    public GameObject CreatePrimitiveWall(Vector3 position, Vector3 scale, Material wallMaterial, Transform levelRootTransform)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = position;
        wall.transform.localScale = scale;
        wall.transform.parent = levelRootTransform; // Parent to the level root for organization
        wall.GetComponent<Renderer>().material = wallMaterial; // Use the assigned wall material
        return wall;
    }

    public void AttachWalls(Room currentLevel, List<GameObject> currentLevelGameObjects, Transform levelRootTransform, Material wallMaterial)
    {
        Room currentRoom = currentLevel;
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
        GameObject bottomWall = CreatePrimitiveWall(bottomWallPos, bottomWallScale , wallMaterial, levelRootTransform);
        bottomWall.name = "BottomWall";
        currentLevelGameObjects.Add(bottomWall);
        
        // Top wall (positive Z)
        Vector3 topWallPos = new Vector3(roomPos.x + roomWidth / 2, 0, roomPos.y + roomDepth + wallThickness / 2);
        Vector3 topWallScale = new Vector3(roomWidth, wallHeight, wallThickness);
        GameObject topWall = CreatePrimitiveWall(topWallPos, topWallScale, wallMaterial, levelRootTransform);
        topWall.name = "TopWall";
        currentLevelGameObjects.Add(topWall);
        
        // Left wall (negative X)
        Vector3 leftWallPos = new Vector3(roomPos.x - wallThickness / 2, 0, roomPos.y + roomDepth / 2);
        Vector3 leftWallScale = new Vector3(wallThickness, wallHeight, roomDepth);
        GameObject leftWall = CreatePrimitiveWall(leftWallPos, leftWallScale, wallMaterial, levelRootTransform);
        leftWall.name = "LeftWall";
        currentLevelGameObjects.Add(leftWall);
        
        // Right wall (positive X)
        Vector3 rightWallPos = new Vector3(roomPos.x + roomWidth + wallThickness / 2, 0, roomPos.y + roomDepth / 2);
        Vector3 rightWallScale = new Vector3(wallThickness, wallHeight, roomDepth);
        GameObject rightWall = CreatePrimitiveWall(rightWallPos, rightWallScale, wallMaterial, levelRootTransform);
        rightWall.name = "RightWall";
        currentLevelGameObjects.Add(rightWall);
    }
    /// <summary>
    /// Visualizes a room in the scene by creating a GameObject with floor and walls
    /// </summary>
    /// <param name="room">The room to visualize</param>

    public void CreateEntrance(Room room, List<GameObject> currentLevelGameObjects, Transform levelRootTransform)
    {
        Vector3 entrancePosition = Room.GetExitPosition(room, room.enteranceDirection);
        GameObject entranceObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        entranceObject.transform.position = entrancePosition;
        entranceObject.transform.localScale = new Vector3(1, 2, 1); // Example size for the entrance
        entranceObject.name = $"Entrance_Direction_{room.enteranceDirection}";
        entranceObject.transform.parent = levelRootTransform;
        currentLevelGameObjects.Add(entranceObject);
    }
    public void CreateExit(Room room, List<GameObject> currentLevelGameObjects, Transform levelRootTransform, GameObject exitDoorPrefab)
    { 
        Vector3 exitPosition = Room.GetExitPosition(room, room.exitDirection);
        GameObject exitObject = Instantiate(exitDoorPrefab, exitPosition, Quaternion.identity, levelRootTransform);
        exitObject.transform.position = exitPosition;
        exitObject.name = $"Exit_Direction_{room.exitDirection}";
        exitObject.transform.parent = levelRootTransform;
        currentLevelGameObjects.Add(exitObject);
    }
}