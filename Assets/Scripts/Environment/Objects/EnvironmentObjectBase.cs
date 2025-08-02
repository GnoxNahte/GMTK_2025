using UnityEngine;

public abstract class EnvironmentObjectBase : MonoBehaviour
{
    public enum EnvType
    {
        // Collectibles
        SmallSpirit,
        LargeSpirit,
        MovementRecover,
        
        // Helping Objects
        Spring,
        MovingPlatform,
        
        // Damaging Objects
        Spikes,
        Stomper,
        
        // Enemies
        Crawler,
        Guard,
        Archer,
    }

    private ObjectPool _pool;
    [field: SerializeField] public EnvType Type { get; private set; }

    public void SetPool(ObjectPool pool)
    {
        _pool = pool;
    }

    public virtual void Release()
    {
        transform.parent = _pool.transform;
        _pool.Release(gameObject);
    }
}
