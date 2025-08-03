using UnityEngine;

public class EnemyBase : EntityBase
{
    [field: SerializeField] public int Damage { get; protected set; }
    private EnvironmentObjectBase _envObj;

    protected override void Awake()
    {
        base.Awake();
        
        _envObj = GetComponent<EnvironmentObjectBase>();
    }

    protected override void OnDead()
    {
        base.OnDead();
        
        _envObj.Release();
    }
}
