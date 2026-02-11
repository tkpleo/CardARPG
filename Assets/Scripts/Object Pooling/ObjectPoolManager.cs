using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class ObjectPoolManager : MonoBehaviour
{
    public static List<PooledObjectInfo> objectPools = new List<PooledObjectInfo>();

    private GameObject _objectPoolEmptyHolder;

    private static GameObject _bulletsEmpty;

    public enum PoolType
    {
        Bullets,
        None
    }
    public static PoolType poolingType;

    private void Awake()
    {
        SetUpEmpties();
    }

    private void SetUpEmpties()
    {
        _objectPoolEmptyHolder = new GameObject("Pooled Objects");

        _bulletsEmpty = new GameObject("Bullets");
        _bulletsEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);
    }

    public static GameObject SpawnObject(GameObject objectSpawn, Vector3 spawnPosition, Quaternion spawnRotation)
        => SpawnObject(objectSpawn, spawnPosition, spawnRotation, 0);

    // New overload: can specify an initialPoolSize to create when the pool is first created
    public static GameObject SpawnObject(GameObject objectSpawn, Vector3 spawnPosition, Quaternion spawnRotation, int initialPoolSize, PoolType poolType = PoolType.None)
    {
        PooledObjectInfo pool = objectPools.Find(p => p.lookupString == objectSpawn.name);
        if (pool == null)
        {
            pool = new PooledObjectInfo() { lookupString = objectSpawn.name };
            objectPools.Add(pool);

            // Preload initial pool size as inactive objects
            GameObject parentForPreloads = SetParentObject(poolType);
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject pre = Object.Instantiate(objectSpawn, Vector3.zero, Quaternion.identity);
                pre.SetActive(false);
                if (parentForPreloads != null)
                {
                    pre.transform.SetParent(parentForPreloads.transform);
                }
                pool.inactiveObjects.Add(pre);
            }
        }

        GameObject spawnableObj = pool.inactiveObjects.FirstOrDefault();

        if (spawnableObj == null)
        {

            GameObject parentObject = SetParentObject(poolType);

            spawnableObj = Object.Instantiate(objectSpawn, spawnPosition, spawnRotation);
            
            if (parentObject != null)
            {
                spawnableObj.transform.SetParent(parentObject.transform);
            }
        }
        else
        {
            spawnableObj.transform.position = spawnPosition;
            spawnableObj.transform.rotation = spawnRotation;
            pool.inactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }

        return spawnableObj;
    }

    // Convenience API: create or ensure a pool for a prefab with an initial size
    public static void CreatePool(GameObject prefab, int initialSize)
    {
        CreatePool(prefab, initialSize, PoolType.None);
    }

    // Overload that allows specifying a PoolType so preloaded instances can be parented correctly
    public static void CreatePool(GameObject prefab, int initialSize, PoolType poolType)
    {
        if (prefab == null || initialSize <= 0) return;

        PooledObjectInfo pool = objectPools.Find(p => p.lookupString == prefab.name);
        if (pool == null)
        {
            pool = new PooledObjectInfo() { lookupString = prefab.name };
            objectPools.Add(pool);
        }

        GameObject parentForPreloads = SetParentObject(poolType);

        // Add the requested number of inactive instances
        for (int i = 0; i < initialSize; i++)
        {
            GameObject pre = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            pre.SetActive(false);
            if (parentForPreloads != null)
            {
                pre.transform.SetParent(parentForPreloads.transform);
            }
            pool.inactiveObjects.Add(pre);
        }
    }

    public static void ReturnObjectToPool(GameObject obj)
    {
        if (obj == null) return;

        // Note: original code strips "(Clone)" by removing last 7 chars.
        // Keep same behavior but guard against unexpected names.
        string goName = obj.name;
        if (goName.EndsWith("(Clone)"))
        {
            goName = goName.Substring(0, goName.Length - 7);
        }

        PooledObjectInfo pool = objectPools.Find(p => p.lookupString == goName);

        if (pool == null)
        {
            Debug.LogWarning("Trying to release an object that is not pooled: " + obj.name);
        }
        else
        {
            // Deactivate and add back to inactive list
            obj.SetActive(false);       
            pool.inactiveObjects.Add(obj);
        }
    }

    private static GameObject SetParentObject(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.Bullets:
                return _bulletsEmpty;

            case PoolType.None:
                return null;
            
            default:
                return null;
        }
    }

}

public class PooledObjectInfo
{
    public string lookupString;
    public List<GameObject> inactiveObjects = new List<GameObject>();
}