using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private InputManager _input;
    private PlayerStats _stats;
    
    public void Init(InputManager inputManager, PlayerStats stats)
    {
        _input = inputManager;
        _stats = stats;
    }
}
