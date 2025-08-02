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

    [field: SerializeField] public EnvType Type { get; private set; }
}
