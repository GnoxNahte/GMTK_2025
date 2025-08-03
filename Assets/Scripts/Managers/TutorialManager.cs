using UnityEngine;
using UnityEngine.SceneManagement;

// Similar as GameManager but for Main Menu
public class TutorialManager : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Player player;
    
    private void Start()
    {
        player.Init(inputManager);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Cutscenes");
    }
}
