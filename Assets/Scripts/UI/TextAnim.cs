using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TextAnim : MonoBehaviour
{
    // Time Delay < 0 to trigger manually
    [SerializeField] private float timeDelay = -1f;
    
    TextMeshPro _textUI;
    string _text = "";
    private bool _hasActivated;

    private void Awake()
    {
        _textUI = GetComponent<TextMeshPro>();
    }

    private IEnumerator Start()
    {
        _text = _textUI.text;
        _textUI.text = "";
        
        if (timeDelay > 0)
        {
            yield return new WaitForSeconds(timeDelay);
            AnimateText();
        }
    }

    public void AnimateText()
    {
        if (_hasActivated)
            return;
        
        StartCoroutine(TypewriterEffect(_text));
        _hasActivated = true;
    }
    
    private IEnumerator TypewriterEffect(string message, float speed = 0.05f) {
        WaitForSeconds wait = new WaitForSeconds(speed);
        
        _textUI.text = ""; // Reset

        foreach (char letter in message) {
            _textUI.text += letter;
            yield return wait;
        }
    }
}
