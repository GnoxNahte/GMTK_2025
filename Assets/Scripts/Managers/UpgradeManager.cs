using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private TextMeshPro requireSpiritText;
    
    private Player _player;
    
    private void Start()
    {
        (GameObject playerObj, LevelManager.LevelData levelData) = levelManager.LoadLevel();
        _player = playerObj.GetComponent<Player>();
        _player.Init(inputManager);
        
        requireSpiritText.text = $"Spirits required to unlock next character:\n{CollectibleUI.CollectedCount}/{LevelManager.RequiredSpirit[LevelManager.SelectedLevel]}";
    }

    public void OnStart()
    {
        SceneManager.LoadScene("Game Scene 2");
    }
}