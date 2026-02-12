using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorGenerator : MonoBehaviour
{
    public List<Node> CreateCorridor(List<RoomNode> allNodesCollection, int corridorWidth)
    {
        List<Node> corridorList = new List<Node>();
        Queue<RoomNode> structuresToCheck = new Queue<RoomNode>(
            allNodesCollection.OrderByDescending(node => node.treeLayerIndex).ToList());

        while(structuresToCheck.Count > 0)
        {
            var node = structuresToCheck.Dequeue();
            if(node.ChildrenNodeList.Count == 0)
            {
                continue;
            }

            CorridorNode corridorNode = new CorridorNode(node.ChildrenNodeList[0], node.ChildrenNodeList[1], corridorWidth);
            corridorList.Add(corridorNode);
        }
        return corridorList;
    }
}
