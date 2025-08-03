using UnityEngine;

// Similar as GameManager but for Main Menu
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Player player;
    
    private void Start()
    {
        player.Init(inputManager);
    }
}
