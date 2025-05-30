using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    void OnGetFromPool();
    void OnReturnToPool();
}

public abstract class PoolableObject : MonoBehaviour, IPoolable
{
    protected ObjectPool<PoolableObject> pool;

    protected virtual void Awake() { }
    protected virtual void OnEnable() { }
    protected virtual void Update() { }

    public virtual void Initialize(ObjectPool<PoolableObject> poolRef)
    {
        pool = poolRef;
        OnGetFromPool();
    }

    public virtual void OnGetFromPool()
    {
        gameObject.SetActive(true);
    }

    public virtual void OnReturnToPool()
    {
        gameObject.SetActive(false);
    }

    protected virtual void ReturnToPool()
    {
        pool?.ReturnToPool(this);
    }
}