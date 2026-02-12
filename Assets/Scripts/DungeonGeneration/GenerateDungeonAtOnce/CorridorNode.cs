using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

/// <summary>
/// Represents a corridor that connects two rooms or structures in the dungeon.
/// Automatically generates corridor geometry to connect two structures based on their relative positions.
/// Corridors can be vertical or horizontal depending on how the structures are arranged.
/// </summary>
public class CorridorNode : Node
{
    /// <summary> The first structure (room or partition) being connected </summary>
    private Node structureOne;
    
    /// <summary> The second structure (room or partition) being connected </summary>
    private Node structureTwo;
    
    /// <summary> The width of the corridor being created </summary>
    private int corridorWidth;
    
    /// <summary> Distance to keep corridors from walls to avoid overlapping exactly with edges </summary>
    private int modifierDistanceFromWall = 1;

    /// <summary>
    /// Constructor to create a corridor between two structures
    /// Automatically calculates corridor position and orientation upon construction
    /// </summary>
    /// <param name="structureOne">First structure to connect</param>
    /// <param name="structureTwo">Second structure to connect</param>
    /// <param name="corridorWidth">Width of the corridor to create</param>
    public CorridorNode(Node structureOne, Node structureTwo, int corridorWidth) : base(null)
    {
        this.structureOne = structureOne;
        this.structureTwo = structureTwo;
        this.corridorWidth = corridorWidth;

        GenerateCorridor();
    }

    /// <summary>
    /// Main method that determines how to connect the two structures based on their relative positions
    /// Chooses between vertical (up/down) or horizontal (left/right) corridor generation
    /// </summary>
    private void GenerateCorridor()
    {
        // Determine if structureTwo is up, down, left, or right of structureOne
        var relativePositionOfStructureTwo = CheckPositionOfStructureTwoAgainstStructureOne();
        switch (relativePositionOfStructureTwo)
        {
            case RelativePosition.Up:
                // structureTwo is above structureOne, create vertical corridor connecting them
                ProcessRoomInRelationUpOrDown(this.structureOne, this.structureTwo);
                break;
            case RelativePosition.Down:
                // structureTwo is below structureOne, create vertical corridor connecting them (reversed order)
                ProcessRoomInRelationUpOrDown(this.structureTwo, this.structureOne);
                break;
            case RelativePosition.Left:
                // structureTwo is to the left of structureOne, create horizontal corridor connecting them
                ProcessRoomInRelationRightOrLeft(this.structureTwo, this.structureOne);
                break;
            case RelativePosition.Right:
                // structureTwo is to the right of structureOne, create horizontal corridor connecting them
                ProcessRoomInRelationRightOrLeft(this.structureOne, this.structureTwo);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Generates a vertical corridor connecting two structures positioned above/below each other
    /// Finds compatible rooms in both structures and calculates the X position for the corridor
    /// </summary>
    /// <param name="structureOne">The lower structure</param>
    /// <param name="structureTwo">The upper structure</param>
    private void ProcessRoomInRelationUpOrDown(Node structureOne, Node structureTwo)
    {
        // Get all leaf nodes (actual rooms) within the lower structure
        Node bottomStructure = null;
        List<Node> structureBottomChildren = StructureHelper.TraverseGraphToExtractLowestLeafs(structureOne);

        // Get all leaf nodes (actual rooms) within the upper structure
        Node topStructure = null;
        List<Node> structureAboveChildren = StructureHelper.TraverseGraphToExtractLowestLeafs(structureTwo);  

        // Find the best room in the bottom structure to connect from
        // Sort by the topmost rooms first (highest Y coordinate)
        var sortedBottomStructures = structureBottomChildren.OrderByDescending(child => child.TopRightAreaCorner.y).ToList();
        if(sortedBottomStructures.Count == 1)
        {
            // Only one room available, use it
            bottomStructure = sortedBottomStructures[0];
        }
        else
        {
            // Multiple rooms available, select from the topmost ones (within 10 units tolerance)
            int maxY = sortedBottomStructures[0].TopLeftAreaCorner.y;
            sortedBottomStructures = sortedBottomStructures.Where(
                child => Mathf.Abs(maxY - child.TopLeftAreaCorner.y) < 10).ToList();

            // Randomly choose from the filtered candidates
            int index = UnityEngine.Random.Range(0, sortedBottomStructures.Count);
            bottomStructure = sortedBottomStructures[index];
        }

        // Find compatible rooms in the top structure that can be reached from the bottom room
        var possibleNeighborsInTopStructure = structureAboveChildren.Where(
            child => GetValidXForNeighborUpDown(
                    bottomStructure.TopLeftAreaCorner,
                    bottomStructure.TopRightAreaCorner,
                    child.BottomLeftAreaCorner,
                    child.BottomRightAreaCorner
                    ) != -1
                ).OrderBy(child => child.BottomRightAreaCorner.y).ToList();
        
        if(possibleNeighborsInTopStructure.Count == 0)
        {
            // No specific room found, use the entire top structure area
            topStructure = structureTwo;
        }
        else
        {
            // Use the first compatible room found
            topStructure = possibleNeighborsInTopStructure[0]; 
        }

        // Calculate the X position where the corridor should run
        int x = GetValidXForNeighborUpDown(
            bottomStructure.TopLeftAreaCorner,
            bottomStructure.TopRightAreaCorner,
            topStructure.BottomLeftAreaCorner,
            topStructure.BottomRightAreaCorner
            );

        // If no valid X position found, try different bottom rooms until one works
        while(x == -1 && sortedBottomStructures.Count > 1)
        {
            // Remove the current room and try the next one
            sortedBottomStructures = sortedBottomStructures.Where(
                child => child.TopLeftAreaCorner.x != topStructure.TopLeftAreaCorner.x).ToList();
            bottomStructure = sortedBottomStructures[0];

            // Recalculate with the new bottom structure
            x = GetValidXForNeighborUpDown(
            bottomStructure.TopLeftAreaCorner,
            bottomStructure.TopRightAreaCorner,
            topStructure.BottomLeftAreaCorner,
            topStructure.BottomRightAreaCorner
            );
        }

        // Set the corridor's corners based on the calculated positions
        BottomLeftAreaCorner = new Vector2Int(x, bottomStructure.TopLeftAreaCorner.y);
        TopRightAreaCorner = new Vector2Int(x + this.corridorWidth, topStructure.BottomLeftAreaCorner.y);
        
    }

    /// <summary>
    /// Calculates a valid X coordinate for a vertical corridor between two structures
    /// Ensures the corridor overlaps with both structures' X ranges
    /// </summary>
    /// <param name="bottomNodeLeft">Left X coordinate of bottom structure</param>
    /// <param name="bottomNodeRight">Right X coordinate of bottom structure</param>
    /// <param name="topNodeLeft">Left X coordinate of top structure</param>
    /// <param name="topNodeRight">Right X coordinate of top structure</param>
    /// <returns>The X coordinate for the corridor, or -1 if no valid position exists</returns>
    private int GetValidXForNeighborUpDown(Vector2Int bottomNodeLeft, Vector2Int bottomNodeRight, 
        Vector2Int topNodeLeft, Vector2Int topNodeRight)
    {
        // Case 1: Top structure is wider and encompasses bottom structure
        if(topNodeLeft.x < bottomNodeLeft.x && bottomNodeRight.x < topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                bottomNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                bottomNodeRight - new Vector2Int(modifierDistanceFromWall + this.corridorWidth, 0)
                ).x;
        }
        
        // Case 2: Bottom structure is wider and encompasses top structure
        if(topNodeLeft.x >= bottomNodeLeft.x && bottomNodeRight.x >= topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                topNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                topNodeRight - new Vector2Int(modifierDistanceFromWall + this.corridorWidth, 0)
                ).x;
        }
        
        // Case 3: Bottom left overlaps with top right area
        if(bottomNodeLeft.x >= topNodeLeft.x && bottomNodeLeft.x <= topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                bottomNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                topNodeRight - new Vector2Int(modifierDistanceFromWall + this.corridorWidth, 0)
                ).x;
        }
        
        // Case 4: Bottom right overlaps with top left area
        if(bottomNodeRight.x <= topNodeRight.x && bottomNodeRight.x >= topNodeLeft.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                topNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                bottomNodeRight - new Vector2Int(modifierDistanceFromWall + this.corridorWidth, 0)
                ).x;
        }
        
        // No valid overlap found
        return -1;
    }

    /// <summary>
    /// Generates a horizontal corridor connecting two structures positioned left/right of each other
    /// Finds compatible rooms in both structures and calculates the Y position for the corridor
    /// </summary>
    /// <param name="structureOne">The left structure</param>
    /// <param name="structureTwo">The right structure</param>
    private void ProcessRoomInRelationRightOrLeft(Node structureOne, Node structureTwo)
    {
        // Get all leaf nodes (actual rooms) within the left structure
        Node leftStructure = null;
        List<Node> leftStructureChildren = StructureHelper.TraverseGraphToExtractLowestLeafs(structureOne);

        // Get all leaf nodes (actual rooms) within the right structure
        Node rightStructure = null;
        List<Node> rightStructureChildren = StructureHelper.TraverseGraphToExtractLowestLeafs(structureTwo);

        // Find the best room in the left structure to connect from
        // Sort by the rightmost rooms first (highest X coordinate)
        var sortedLeftStructures = leftStructureChildren.OrderByDescending(child => child.TopRightAreaCorner.x).ToList();
        if(sortedLeftStructures.Count > 1)
        {
            // Multiple rooms, select from the rightmost ones
            leftStructure = sortedLeftStructures[0];
        }
        else
        {
            // Filter to rooms near the max X within 10 units tolerance
            int maxX = sortedLeftStructures[0].TopRightAreaCorner.x;
            sortedLeftStructures = sortedLeftStructures.Where(children => Mathf.Abs(maxX - children.TopRightAreaCorner.x) < 10).ToList();
            
            // Randomly choose from the filtered candidates
            int index = UnityEngine.Random.Range(0, sortedLeftStructures.Count);
            leftStructure = sortedLeftStructures[index];
        }

        // Find compatible rooms in the right structure that can be reached from the left room
        var possibleNeighborsInRightStructure = rightStructureChildren.Where(
            child => GetValidYForNeighborLeftRight(
                    leftStructure.TopRightAreaCorner,
                    leftStructure.BottomRightAreaCorner,
                    child.TopLeftAreaCorner,
                    child.BottomLeftAreaCorner
                    ) != -1
                ).OrderBy(child => child.BottomRightAreaCorner.x).ToList();

        if(possibleNeighborsInRightStructure.Count <= 0)
        {
            // No specific room found, use the entire right structure area
            rightStructure = structureTwo;
        }
        else
        {
            // Use the first compatible room found
            rightStructure = possibleNeighborsInRightStructure[0];
        }

        // Calculate the Y position where the corridor should run
        int y = GetValidYForNeighborLeftRight(
            leftStructure.TopRightAreaCorner,
            leftStructure.BottomRightAreaCorner,
            rightStructure.TopLeftAreaCorner,
            rightStructure.BottomLeftAreaCorner
            );

        // If no valid Y position found, try different left rooms until one works
        while(y == -1 && sortedLeftStructures.Count > 1)
        {
            // Remove the current room and try the next one
            sortedLeftStructures = sortedLeftStructures.Where(child => child.TopLeftAreaCorner.y
                != leftStructure.TopLeftAreaCorner.y).ToList();
            leftStructure = sortedLeftStructures[0];

            // Recalculate with the new left structure
            y = GetValidYForNeighborLeftRight(
                leftStructure.TopRightAreaCorner,
                leftStructure.BottomRightAreaCorner,
                rightStructure.TopLeftAreaCorner,
                rightStructure.BottomLeftAreaCorner
                );
        }

        // Set the corridor's corners based on the calculated positions
        BottomLeftAreaCorner = new Vector2Int(leftStructure.BottomRightAreaCorner.x, y);
        TopRightAreaCorner = new Vector2Int(rightStructure.TopLeftAreaCorner.x, y + this.corridorWidth);
    }

    /// <summary>
    /// Calculates a valid Y coordinate for a horizontal corridor between two structures
    /// Ensures the corridor overlaps with both structures' Y ranges
    /// </summary>
    /// <param name="leftNodeUp">Top Y coordinate of left structure</param>
    /// <param name="leftNodeDown">Bottom Y coordinate of left structure</param>
    /// <param name="rightNodeUp">Top Y coordinate of right structure</param>
    /// <param name="rightNodeDown">Bottom Y coordinate of right structure</param>
    /// <returns>The Y coordinate for the corridor, or -1 if no valid position exists</returns>
    private int GetValidYForNeighborLeftRight(Vector2Int leftNodeUp, Vector2Int leftNodeDown, 
        Vector2Int rightNodeUp, Vector2Int rightNodeDown)
    {
        // Case 1: Right structure is taller and encompasses left structure
        if(rightNodeUp.y >= leftNodeUp.y && leftNodeDown.y >= rightNodeDown.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                leftNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                leftNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)).y;
        }
        
        // Case 2: Left structure is taller and encompasses right structure
        if(rightNodeUp.y <= leftNodeUp.y && leftNodeDown.y <= rightNodeDown.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                rightNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                rightNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)).y;
        }
        
        // Case 3: Left top overlaps with right middle area
        if(leftNodeUp.y >= rightNodeDown.y && leftNodeUp.y <= rightNodeUp.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                rightNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                leftNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)).y;
        }
        
        // Case 4: Left bottom overlaps with right middle area
        if(leftNodeDown.y >= rightNodeDown.y && leftNodeDown.y <= rightNodeUp.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                leftNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                rightNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)).y;
        }
        
        // No valid overlap found
        return -1;
    }

    /// <summary>
    /// Determines the relative position of structureTwo compared to structureOne
    /// Uses angle calculation between structure centers to classify direction into four quadrants
    /// </summary>
    /// <returns>The relative position (Up, Down, Left, or Right)</returns>
    private RelativePosition CheckPositionOfStructureTwoAgainstStructureOne()
    {
        // Calculate the center points of both structures
        Vector2 middlePointStructureOneTemp = ((Vector2)structureOne.TopRightAreaCorner + structureOne.BottomLeftAreaCorner) / 2;
        Vector2 middlePointStructureTwoTemp = ((Vector2)structureTwo.TopRightAreaCorner + structureTwo.BottomLeftAreaCorner) / 2;

        // Calculate the angle between the two centers
        float angle = CalculateAngle(middlePointStructureOneTemp, middlePointStructureTwoTemp);

        // Classify direction based on angle ranges
        if((angle < 45 && angle >= 0) || (angle > -45 && angle < 0))
        {
            return RelativePosition.Right;  // 0° (right)
        }
        else if((angle > 45 && angle < 135))
        {
            return RelativePosition.Up;     // 90° (up)
        }
        else if((angle < -45 && angle > -135))
        {
            return RelativePosition.Down;   // -90° (down)
        }
        else
        {
            return RelativePosition.Left;   // 180° or -180° (left)
        }
    }

    /// <summary>
    /// Calculates the angle in degrees from one point to another
    /// Uses arctangent to determine the direction vector
    /// </summary>
    /// <param name="middlePointStructureOneTemp">Starting point</param>
    /// <param name="middlePointStructureTwoTemp">Target point</param>
    /// <returns>Angle in degrees (-180 to 180)</returns>
    private float CalculateAngle(Vector2 middlePointStructureOneTemp, Vector2 middlePointStructureTwoTemp)
    {
        return Mathf.Atan2(middlePointStructureTwoTemp.y - middlePointStructureOneTemp.y,
            middlePointStructureTwoTemp.x - middlePointStructureOneTemp.x) * Mathf.Rad2Deg;
    }
}
