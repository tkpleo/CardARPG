using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Optional interface pooled objects can implement to request return to the pool
/// and to receive a notification when they are taken from the pool.
/// </summary>
public interface IPoolable<TItem>
    where TItem : MonoBehaviour
{
    /// <summary>
    /// Raise this event when the object should be returned to the factory (e.g. on collision or death).
    /// The factory will subscribe to this and handle enqueueing/destruction.
    /// </summary>
    event Action<TItem> ReturnRequested;

    /// <summary>
    /// Called by the factory when the object is taken from the pool. Use to reset transient state.
    /// </summary>
    void OnRetrieved();
}

public abstract class ObjectFactory<TFactory, TItem> : Singleton<TFactory>
    where TFactory : ObjectFactory<TFactory, TItem>
    where TItem : MonoBehaviour
{
    private readonly Dictionary<TItem, Queue<TItem>> _objectPools = new();
    // fast reverse lookup from instance -> prefab (avoids scanning queues)
    private readonly Dictionary<TItem, TItem> _instanceToPrefab = new();

    protected override string ObjectName => $"{typeof(TItem).Name} Object Factory and Pooler";

    #region Public API
    /// <summary>
    /// Instantiates a specified number of objects based on the given prefab and adds them to the pool for future use.
    /// </summary>
    /// <param name="prefab">The prefab object to use as a template for creating new instances. Cannot be null.</param>
    /// <param name="count">The number of instances to create and add to the pool. Must be non-negative.</param>
    public static void Prewarm(TItem prefab, int count)
    {
        int existing = Instance._objectPools.TryGetValue(prefab, out var pool) ? pool.Count : 0;
        int difference = count - existing;

        for (int i = 0; i < difference; i++)
            Instance.CreateObject(prefab);
    }

    public static TItem GetObject(TItem prefab)
    {
        if (!Instance._objectPools.TryGetValue(prefab, out Queue<TItem> pool) || pool.Count < 1)
            Instance.CreateObject(prefab); // Creates a new object if the pool is empty

        TItem obj = Instance._objectPools[prefab].Dequeue() ?? throw new InvalidOperationException($"Failed to retrieve an object from the pool for prefab: {prefab}");

        obj.gameObject.SetActive(true);

        if (obj is IPoolable<TItem> poolable)
            poolable.OnRetrieved();

        return obj;
    }
    #endregion

    protected TItem CreateObject(TItem prefab)
    {
        TItem obj = Instantiate(prefab); // Creates a new instance of the prefab
        obj.gameObject.SetActive(false);

        if (!_objectPools.ContainsKey(prefab))
            _objectPools[prefab] = new Queue<TItem>();

        _objectPools[prefab].Enqueue(obj);

        // Track reverse mapping so ReturnObject doesn't need to scan all queues.
        _instanceToPrefab[obj] = prefab;

        // If the instance implements IPoolable<TItem>, subscribe so it can request returns.
        if (obj is IPoolable<TItem> poolable)
            poolable.ReturnRequested += ReturnObject;

        return obj;
    }

    /// <summary>
    /// Called when a pooled instance requests that it be returned to the pool.
    /// Objects should raise the IPoolable.ReturnRequested event when appropriate (collision, death, etc).
    /// </summary>
    private void ReturnObject(TItem obj)
    {
        // Basic guard
        if (obj == null) return;

        // Deactivate first so any return-time visuals/logic don't run while pooled.
        obj.gameObject.SetActive(false);

        if (_instanceToPrefab.TryGetValue(obj, out TItem prefab) && _objectPools.TryGetValue(prefab, out var pool))
        {
            pool.Enqueue(obj);
            return;
        }

        // If we don't recognise the instance, unsubscribe (if possible) and destroy it.
        if (obj is IPoolable<TItem> poolable)
            poolable.ReturnRequested -= ReturnObject;

        Debug.LogWarning($"Returned object {obj} does not belong to any pool. It will be destroyed.");
        Destroy(obj.gameObject);
    }
}
