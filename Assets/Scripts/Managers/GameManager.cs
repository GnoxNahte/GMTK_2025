using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Player player;
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private GameUIManager gameUIManager;
    
    private void Start()
    {
        player.Init(inputManager);
        mapGenerator.Init(player);
        gameUIManager.Init(player);
    }
}
