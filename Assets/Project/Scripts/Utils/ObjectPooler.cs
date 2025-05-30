using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour, IPoolable
{
    private readonly Queue<T> availableObjects = new Queue<T>();
    private readonly HashSet<T> allObjects = new HashSet<T>();
    private readonly T prefab;
    private readonly Transform parent;
    private readonly int maxSize;

    public int ActiveCount => allObjects.Count - availableObjects.Count;
    public int AvailableCount => availableObjects.Count;

    public ObjectPool(T prefab, int initialSize, int maxSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.maxSize = maxSize;
        this.parent = parent;

        PrewarmPool(initialSize);
    }

    private void PrewarmPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            T obj = CreateNewObject();
            obj.OnReturnToPool();
            availableObjects.Enqueue(obj);
        }
    }

    private T CreateNewObject()
    {
        T obj = Object.Instantiate(prefab, parent);
        allObjects.Add(obj);
        return obj;
    }

    public T Get()
    {
        T obj;

        if (availableObjects.Count > 0)
        {
            obj = availableObjects.Dequeue();
        }
        else if (allObjects.Count < maxSize)
        {
            obj = CreateNewObject();
        }
        else
        {
            // Pool is full, reuse oldest active object
            return null;
        }

        obj.OnGetFromPool();
        return obj;
    }

    public void ReturnToPool(T obj)
    {
        if (obj == null || availableObjects.Contains(obj)) return;

        obj.OnReturnToPool();
        availableObjects.Enqueue(obj);
    }

    public void Clear()
    {
        foreach (T obj in allObjects)
        {
            if (obj != null)
                Object.Destroy(obj.gameObject);
        }

        availableObjects.Clear();
        allObjects.Clear();
    }
}

