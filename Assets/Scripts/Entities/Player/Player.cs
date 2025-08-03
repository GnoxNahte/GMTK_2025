using System;
using UnityEngine;

public class Player : EntityBase
{
    [field: SerializeField] public PlayerStats Stats  { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public PlayerVisuals Visuals { get; private set; }
    
    public void Init(InputManager inputManager)
    {
        Movement.Init(inputManager, Stats);
        Visuals.Init(Movement);
    }

    protected override void Awake()
    {
        base.Awake();
        
        Movement = GetComponent<PlayerMovement>();
        Visuals = GetComponentInChildren<PlayerVisuals>();
    }

    private void OnEnable() => Movement.OnHit += TakeDamage;
    private void OnDisable() => Movement.OnHit -= TakeDamage;
}
