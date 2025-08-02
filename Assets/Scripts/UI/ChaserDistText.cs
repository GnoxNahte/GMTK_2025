using System;
using TMPro;
using UnityEngine;

public class ChaserDistText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Chaser chaser;
    [SerializeField] private Player player;
    [SerializeField] private float padding;

    private Camera _camera;
    private Canvas _canvas;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        float xPos = Mathf.Max(_camera.WorldToScreenPoint(chaser.transform.position).x / _canvas.scaleFactor + padding, 0);
        _rectTransform.anchoredPosition = new Vector2(xPos, _rectTransform.anchoredPosition.y);
        text.text = $"{(int)(player.transform.position.x - chaser.transform.position.x)}m";
    }
}
