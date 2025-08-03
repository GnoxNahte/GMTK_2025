using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Cutscenes : MonoBehaviour
{
    [System.Serializable]
    public class Cutscene
    {
        public GameObject[] Cutscenes;
    }
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Cutscene[] cutscenes;
    private int _index = 0;

    private void OnEnable() => inputManager.OnContinuePressed += OnContinuePressed;
    private void OnDisable() => inputManager.OnContinuePressed -= OnContinuePressed;

    private void Awake()
    {
        OnContinuePressed();
    }

    private void OnContinuePressed()
    {
        if (LevelManager.SelectedLevel + 1 >= LevelManager.MaxLevel)
            return;
        
        if (_index >= cutscenes.Length)
            SceneManager.LoadScene("Game Scene 2");
        else
            cutscenes[LevelManager.SelectedLevel].Cutscenes[_index++].SetActive(true);
    }
}
