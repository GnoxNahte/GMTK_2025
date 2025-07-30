using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    #region Public Variables
    [field: SerializeField, ReadOnly] public Vector2 MoveDir { get; private set; }
    #endregion
    
    #region Private Variables
    private PlayerControls _playerControls;
    
    private InputAction _move;
    private InputAction _dash;
    private InputAction _attack;
    private InputAction _pause;
    
    private PlayerMovement _playerMovement;
    #endregion

    #region Public Methods

    public void Init(PlayerMovement playerMovement)
    {
        _playerMovement = playerMovement;
    }

    #endregion
    
    #region Unity Methods
    private void Awake()
    {
        _playerControls = new PlayerControls();
        
        _move = _playerControls.Player.Move;
        _dash = _playerControls.Player.Dash;
        _attack = _playerControls.Player.Attack;
        _pause = _playerControls.Player.Pause;
    }

    private void OnEnable()
    {
        _move.Enable();
        _dash.Enable();
        _attack.Enable();
        _pause.Enable();
    }

    private void OnDisable()
    {
        _move.Disable();
        _dash.Disable();
        _attack.Disable();
        _pause.Disable();
    }

    private void Update()
    {
        MoveDir = _move.ReadValue<Vector2>();
        
        // === Abilities ===
        // if (_dash.IsPressed())
        //     _playerMovement.Dash();
    }
    #endregion
}