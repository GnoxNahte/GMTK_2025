using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private GameUIManager gameUIManager;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private Chaser chaser;
    
    private Player _player;
    
    private void Start()
    {
        (GameObject playerObj, LevelManager.LevelData levelData) = levelManager.LoadLevel();
        _player = playerObj.GetComponent<Player>();
        _player.Init(inputManager);
        cameraManager.Init(_player);
        mapGenerator.Init(_player, levelData.SourceOverrideTile, levelData.OverrideTile);
        gameUIManager.Init(_player, chaser);
    }
}
