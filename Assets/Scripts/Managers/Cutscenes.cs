using System;
using UnityEngine;

public class Cutscenes : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;

    private void OnEnable() => inputManager.OnContinuePressed += OnContinuePressed;
    private void OnDisable() => inputManager.OnContinuePressed -= OnContinuePressed;
    
    

    private void OnContinuePressed()
    {
        
    }
}
