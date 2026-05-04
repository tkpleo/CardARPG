using UnityEngine;
using System.Collections.Generic;

public class GapCreator
{
    private const string gapTag = "Gap";

    public static void MakeRandomGapsInFloor(Vector2Int bottomLeftCorner, Vector2Int topRightCorner, float offsetFromWalls, Room room,
     List<GameObject> currentLevelGameObjects, Transform levelRootTransform, int gapWidthMin, int gapWidthMax, 
     int gapLengthMin, int gapLengthMax, int minGapCount, int maxGapCount, Material gapMaterial, List<GameObject> gaps)
    {
        var gapCount = Random.Range(minGapCount, maxGapCount + 1);
        for (int i = 0; i < gapCount; i++)
        {
            Vector2Int exitPos = room.GetExitPosition();
            Vector2Int entrancePos = room.GetEntrancePosition();
            Vector3 gapScale = new Vector3(Random.Range(gapWidthMin, gapWidthMax + 1), 0.02f, Random.Range(gapLengthMin, gapLengthMax + 1));
            Vector2 validPosition = FindValidGapPosition(bottomLeftCorner, topRightCorner, offsetFromWalls, exitPos, entrancePos, gapScale, gaps);
            Vector3 gapPosition = new Vector3(validPosition.x, 0.01f, validPosition.y);
            GameObject gap = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gap.transform.position = gapPosition;
            gap.transform.localScale = gapScale;
            gap.GetComponent<Renderer>().material = gapMaterial;
            BoxCollider boxCollider = gap.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(1, 50, 1);
            boxCollider.center = new Vector3(0, 25, 0);
            gap.name = "Gap_" + i;
            currentLevelGameObjects.Add(gap);
            gap.transform.parent = levelRootTransform;
            gap.transform.tag = gapTag;
            gaps.Add(gap);
        }
    }

    private static Vector2 FindValidGapPosition(Vector2Int bottomLeftCorner, Vector2Int topRightCorner, float offsetFromWalls, 
        Vector2Int exitPos, Vector2Int entrancePos, Vector3 gapScale, List<GameObject> existingGaps)
    {
        int maxAttempts = 50;
        float halfWidth = gapScale.x / 2f;
        float halfLength = gapScale.z / 2f;
        float minX = bottomLeftCorner.x + offsetFromWalls + halfWidth;
        float maxX = topRightCorner.x - offsetFromWalls - halfWidth;
        float minY = bottomLeftCorner.y + offsetFromWalls + halfLength;
        float maxY = topRightCorner.y - offsetFromWalls - halfLength;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);
            if (IsValidGapPosition(randomX, randomY, gapScale, exitPos, entrancePos, existingGaps))
            {
                return new Vector2(randomX, randomY);
            }
        }
        Debug.LogWarning("Could not find non-overlapping position for gap after " + maxAttempts + " attempts");
        return new Vector2(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY)
        );
    }

    private static bool IsValidGapPosition(float posX, float posY, Vector3 gapScale, Vector2Int exitPos, Vector2Int entrancePos, List<GameObject> existingGaps, float gapOffsetFromEachOther = 1f)
    {
        float exitPadding = 2f;
        if (Mathf.Abs(posX - exitPos.x) < (gapScale.x / 2 + exitPadding) &&
            Mathf.Abs(posY - exitPos.y) < (gapScale.z / 2 + exitPadding))
        {
            return false;
        }
        if (Mathf.Abs(posX - entrancePos.x) < (gapScale.x / 2 + exitPadding) &&
            Mathf.Abs(posY - entrancePos.y) < (gapScale.z / 2 + exitPadding))
        {
            return false;
        }
        foreach (GameObject existingGap in existingGaps)
        {
            Vector3 otherPos = existingGap.transform.position;
            Vector3 otherScale = existingGap.transform.localScale;
            float padding = gapOffsetFromEachOther;
            if (Mathf.Abs(posX - otherPos.x) < (gapScale.x + otherScale.x) / 2 + padding &&
                Mathf.Abs(posY - otherPos.z) < (gapScale.z + otherScale.z) / 2 + padding)
            {
                return false;
            }
        }
        return true;
    }
}

