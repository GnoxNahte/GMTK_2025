using UnityEngine;

public class Spike : EnvironmentObjectBase
{
    [field: SerializeField] public Vector2 KnockbackStrength { get; private set; }
    [field: SerializeField] public int Damage { get; private set; }
}
