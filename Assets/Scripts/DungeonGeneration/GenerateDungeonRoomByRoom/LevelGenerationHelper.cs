using UnityEngine;
using System.Collections.Generic;
public class LevelGenerationHelper : MonoBehaviour
{
    public static void DestroyAllChildren(GameObject parent)
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

    public static void CleanUpPreviousLevel(List<GameObject> currentLevelGameObjects, Transform levelRootTransform)
    {
        // Ensure the list is initialized
        if (currentLevelGameObjects == null)
            currentLevelGameObjects = new List<GameObject>();
        // Clean up existing GameObjects from previous levels
        foreach(GameObject obj in currentLevelGameObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            } 
            else
            {
                Debug.LogWarning("Attempted to destroy a null GameObject reference during cleanup.");
            }
        }
        
        currentLevelGameObjects.Clear();

        // Also destroy children under levelRootTransform for safety
        if (levelRootTransform != null && levelRootTransform.childCount > 0)
        {
            DestroyAllChildren(levelRootTransform.gameObject);
        }
    }
}
