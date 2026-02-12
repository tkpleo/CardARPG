using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main controller for dungeon generation and visualization in Unity.
/// Handles dungeon generation, mesh creation for floors, and wall placement.
/// This is the MonoBehaviour that should be attached to a GameObject in the scene.
/// </summary>
public class DungeonCreator : MonoBehaviour
{
    // ===== Dungeon Generation Parameters =====
    /// <summary> The width of the entire dungeon to generate </summary>
    [SerializeField] private int dungeonWidth;
    
    /// <summary> The length/height of the entire dungeon to generate </summary>
    [SerializeField] private int dungeonLength;
    [SerializeField] private int roomWidthMin;
    [SerializeField] private int roomLengthMin;
    [SerializeField] private int maxIterations;
    [SerializeField] private int corridorWidth;
    [SerializeField, Range(0.0f, 0.3f)] private float roomBottomCornerModifier;
    [SerializeField, Range(0.7f, 1f)] private float roomTopCornerModifier;
    [SerializeField, Range(0, 2)] private int roomOffset;
    [SerializeField] private Material material;
    [SerializeField] private GameObject wallVertical;
    [SerializeField] GameObject wallHorizontal;
    private List<Vector3Int> possibleDoorVerticalPosition;
    private List<Vector3Int> possibleDoorHorizontalPosition;
    private List<Vector3Int> possibleWallVerticalPosition;
    private List<Vector3Int> possibleWallHorizontalPosition;

    public void Start()
    {
        CreateDungeon();
    }

    public void CreateDungeon()
    {
        DestroyAllChildren();
        DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonLength);
        
        var listOfRooms = generator.CalculateDungeon(maxIterations, roomWidthMin, roomLengthMin,
            roomBottomCornerModifier, roomTopCornerModifier, roomOffset, corridorWidth);

        GameObject wallParent = new GameObject("WallParent");

        wallParent.transform.parent = transform;

        possibleDoorHorizontalPosition = new List<Vector3Int>();
        possibleDoorVerticalPosition = new List<Vector3Int>();
        possibleWallHorizontalPosition = new List<Vector3Int>();
        possibleWallVerticalPosition = new List<Vector3Int>();

        for(int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMeshes(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
        }

        AttachWalls(wallParent);
    }

    private void AttachWalls(GameObject wallParent)
    {
        foreach(var wallPosition in possibleWallHorizontalPosition)
        {
            CreateWalls(wallParent, wallPosition, wallHorizontal);
        }
        foreach(var wallPosition in possibleWallVerticalPosition)
        {
            CreateWalls(wallParent, wallPosition, wallVertical);
        }
    }

    private void CreateWalls(GameObject wallParent, Vector3Int wallPosition, GameObject wallPrefab)
    {
        Instantiate(wallPrefab, wallPosition, Quaternion.identity, wallParent.transform);
    }

    private void CreateMeshes(Vector2 bottomLeftCorner, Vector2 topRightCorner)
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

        dungeonFloor.transform.parent = transform;

        dungeonFloor.transform.position = Vector3.zero;
        dungeonFloor.transform.localScale = Vector3.one;

        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = material;

        for(int row = (int)bottomLeftV.x; row < (int)bottomRightV.x; row++)
        {
            var wallPosition = new Vector3(row, 0, bottomLeftV.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalPosition);
        }

        for(int row = (int)topLeftV.x; row < (int)topRightV.x; row++)
        {
            var wallPosition = new Vector3(row, 0, topRightV.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalPosition);
        }

        for(int col = (int)bottomLeftV.z; col < (int)topLeftV.z; col++)
        {
            var wallPosition = new Vector3(bottomLeftV.x, 0, col);
            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
        }

        for(int col = (int)bottomRightV.z; col < (int)topRightV.z; col++)
        {
            var wallPosition = new Vector3(bottomRightV.x, 0, col);
            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
        }
    }

    private void AddWallPositionToList(Vector3 wallPosition, List<Vector3Int> wallList, List<Vector3Int> doorList)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPosition);
        if (wallList.Contains(point))
        {
            doorList.Add(point);
            wallList.Remove(point);
        }
        else
        {
            wallList.Add(point);
        }
    }

    private void DestroyAllChildren()
    {
        while(transform.childCount != 0)
        {
            foreach(Transform item in transform)
            {
                DestroyImmediate(item.gameObject);
            }
        }
    }
}
