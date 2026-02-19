using UnityEngine;
using System.Collections.Generic;

public class GapCreator
{
    public static void MakeRandomGapsInFloor(Vector2Int bottomLeftCorner, Vector2Int topRightCorner, float offsetFromWalls, Room room,
     List<GameObject> currentLevelGameObjects, Transform levelRootTransform, int gapWidthMin, int gapWidthMax, 
     int gapLengthMin, int gapLengthMax, int minGapCount, int maxGapCount, Material gapMaterial, List<GameObject> gaps)
    {
        gaps = new List<GameObject>();
        var gapCount = Random.Range(minGapCount, maxGapCount + 1);
        for (int i = 0; i < gapCount; i++)
        {
            Vector2Int exitPos = room.GetExitPosition();
            Vector3 gapScale = new Vector3(Random.Range(gapWidthMin, gapWidthMax + 1), 0.02f, Random.Range(gapLengthMin, gapLengthMax + 1));
            Vector2 validPosition = FindValidGapPosition(bottomLeftCorner, topRightCorner, offsetFromWalls, exitPos, gapScale, gaps);
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
            gaps.Add(gap);
        }
    }

    private static Vector2 FindValidGapPosition(Vector2Int bottomLeftCorner, Vector2Int topRightCorner, float offsetFromWalls, 
        Vector2Int exitPos, Vector3 gapScale, List<GameObject> existingGaps)
    {
        int maxAttempts = 50;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            float randomX = Random.Range(bottomLeftCorner.x + offsetFromWalls, topRightCorner.x - offsetFromWalls);
            float randomY = Random.Range(bottomLeftCorner.y + offsetFromWalls, topRightCorner.y - offsetFromWalls);
            if (IsValidGapPosition(randomX, randomY, gapScale, exitPos, existingGaps))
            {
                return new Vector2(randomX, randomY);
            }
        }
        Debug.LogWarning("Could not find non-overlapping position for gap after " + maxAttempts + " attempts");
        return new Vector2(
            Random.Range(bottomLeftCorner.x + offsetFromWalls, topRightCorner.x - offsetFromWalls),
            Random.Range(bottomLeftCorner.y + offsetFromWalls, topRightCorner.y - offsetFromWalls)
        );
    }

    private static bool IsValidGapPosition(float posX, float posY, Vector3 gapScale, Vector2Int exitPos, List<GameObject> existingGaps, float gapOffsetFromEachOther = 1f)
    {
        float exitPadding = 2f;
        if (Mathf.Abs(posX - exitPos.x) < (gapScale.x / 2 + exitPadding) &&
            Mathf.Abs(posY - exitPos.y) < (gapScale.z / 2 + exitPadding))
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

