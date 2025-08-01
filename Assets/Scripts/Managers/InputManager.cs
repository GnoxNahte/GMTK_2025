using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using VInspector;

public class InputManager : MonoBehaviour
{
    [field: SerializeField, ReadOnly] public Vector2 MoveDir { get; private set; }
    [field: SerializeField, ReadOnly] public bool IsJumping { get; private set; }
    [field: SerializeField, ReadOnly] public bool JumpPressedThisFrame { get; private set; }
    
    private PlayerControls _playerControls;
    
    private InputAction _move;
    private InputAction _jump;
    private InputAction _dash;
    private InputAction _attack;
    private InputAction _pause;
    private InputAction _continue;
    
    public event Action OnDashPressed;
    public event Action OnContinuePressed;
    
    #region Unity Methods
    private void Awake()
    {
        _playerControls = new PlayerControls();
        
        _move = _playerControls.Player.Move;
        _jump = _playerControls.Player.Jump;
        _dash = _playerControls.Player.Dash;
        _attack = _playerControls.Player.Attack;
        _pause = _playerControls.Player.Pause;
        _continue = _playerControls.Player.Continue;
    }

    private void OnEnable()
    {
        _move.Enable();
        _jump.Enable();
        _dash.Enable();
        _attack.Enable();
        _pause.Enable();
        _continue.Enable();
    }

    private void OnDisable()
    {
        _move.Disable();
        _jump.Disable();
        _dash.Disable();
        _attack.Disable();
        _pause.Disable();
        _continue.Disable();
    }

    private void Update()
    {
        MoveDir = _move.ReadValue<Vector2>();
        IsJumping = _jump.IsPressed();
        JumpPressedThisFrame = _jump.WasPressedThisFrame();
        
        // === Abilities ===
        if (_dash.IsPressed())
            OnDashPressed?.Invoke();
        
        // Others
        if (_continue.WasPressedThisFrame())
            OnContinuePressed?.Invoke();
    }
    #endregion
}
