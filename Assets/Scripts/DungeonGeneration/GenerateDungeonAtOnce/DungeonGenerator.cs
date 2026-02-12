using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Main orchestrator for dungeon generation using Binary Space Partitioning (BSP) algorithm.
/// Coordinates the generation of room spaces and corridors connecting them.
/// </summary>
public class DungeonGenerator
{
    /// <summary> Collection of all nodes in the binary space partition tree </summary>
    List<RoomNode> allNodesCollection = new List<RoomNode>();
    
    /// <summary> The width of the entire dungeon area </summary>
    private int dungeonWidth;
    
    /// <summary> The length/height of the entire dungeon area </summary>
    private int dungeonLength;

    /// <summary>
    /// Constructor to initialize the dungeon generator with dimensions
    /// </summary>
    /// <param name="dungeonWidth">The width of the dungeon to generate</param>
    /// <param name="dungeonLength">The length/height of the dungeon to generate</param>
    public DungeonGenerator(int dungeonWidth, int dungeonLength)
    {
        this.dungeonWidth = dungeonWidth;
        this.dungeonLength = dungeonLength;
    }

    /// <summary>
    /// Main method to calculate and generate the entire dungeon structure
    /// Performs BSP partitioning, then generates rooms and corridors
    /// </summary>
    /// <param name="maxIterations">Maximum number of times to split the space</param>
    /// <param name="roomWidthMin">Minimum width for a room in the BSP process</param>
    /// <param name="roomLengthMin">Minimum length/height for a room in the BSP process</param>
    /// <param name="roomBottomCornerModifier">Percentage modifier (0-0.3) for bottom-left corner positioning</param>
    /// <param name="roomTopCornerModifier">Percentage modifier (0.7-1) for top-right corner positioning</param>
    /// <param name="roomOffset">Distance to offset room corners from the partition boundaries</param>
    /// <param name="corridorWidth">The width of corridors connecting rooms</param>
    /// <returns>A combined list of all room and corridor nodes in the dungeon</returns>
    public List<Node> CalculateDungeon(int maxIterations, int roomWidthMin, int roomLengthMin, 
        float roomBottomCornerModifier, float roomTopCornerModifier, int roomOffset, int corridorWidth)
    {
        // Create the BSP tree by partitioning the dungeon space
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(dungeonWidth, dungeonLength);
        allNodesCollection = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);
        
        // Extract the leaf nodes (actual room spaces) from the BSP tree
        List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeafs(bsp.RootNode);

        // Generate actual rooms within the partitioned spaces
        RoomGenerator roomGenerator = new RoomGenerator(maxIterations, roomLengthMin, roomWidthMin);
        List<RoomNode> roomList = roomGenerator.GenerateRoomsInGivenSpaces(roomSpaces, roomBottomCornerModifier, 
            roomTopCornerModifier, roomOffset);

        // Generate corridors that connect the rooms together
        CorridorGenerator corridorGenerator = new CorridorGenerator();
        var corridorList = corridorGenerator.CreateCorridor(allNodesCollection, corridorWidth);

        // Combine and return both rooms and corridors
        return new List<Node>(roomList).Concat(corridorList).ToList();
    }
}
