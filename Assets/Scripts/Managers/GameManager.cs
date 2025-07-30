using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Player player;
    
    private void Start()
    {
        inputManager.Init(player.Movement);
        player.Init(inputManager);
    }
}
