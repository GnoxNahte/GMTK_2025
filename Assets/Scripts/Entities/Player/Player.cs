using UnityEngine;

public class Player : EntityBase
{
    [field: SerializeField] public PlayerStats Stats  { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public PlayerVisuals Visuals { get; private set; }
    
    public void Init(InputManager inputManager)
    {
        Movement.Init(inputManager, Stats);
    }

    protected override void Awake()
    {
        base.Awake();
        
        Movement = GetComponent<PlayerMovement>();
        Visuals = GetComponentInChildren<PlayerVisuals>();
    }
}
