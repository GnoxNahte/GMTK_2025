using System;
using UnityEngine;
using UnityEngine.UI;

public class Cutscenes : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Image[] cutscenes;

    private void OnEnable() => inputManager.OnContinuePressed += OnContinuePressed;
    private void OnDisable() => inputManager.OnContinuePressed -= OnContinuePressed;

    private void OnContinuePressed()
    {
        
    }
}
