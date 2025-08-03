using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeUIAnim : MonoBehaviour
{
    private Image _image;
    [SerializeField] private bool toFadeIn;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void Start()
    {
        Fade(null);
    }

    public void Fade(Action onComplete, float duration = 1f)
    {
        StartCoroutine(FadeAnim(toFadeIn ? Color.white : new Color(0, 0, 0, 0), onComplete, duration));
    }

    private IEnumerator FadeAnim(Color targetColor, Action onComplete, float duration = 1f)
    {
        float animDuration = 0;

        Color startColor = _image.color;
        while (animDuration < duration)
        {
            _image.color = Color.Lerp(startColor, targetColor, animDuration / duration);
            animDuration += Time.deltaTime;
            yield return null;
        }
        
        onComplete?.Invoke();
    }
}
