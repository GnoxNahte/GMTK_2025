using System;
using TMPro;
using UnityEngine;

public class ChaserDistText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float padding;

    private Chaser _chaser;
    private Player _player;
    private Camera _camera;
    private Canvas _canvas;
    private RectTransform _rectTransform;

    public void Init(Player player, Chaser chaser)
    {
        _player = player;
        _chaser = chaser;
    }

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
        float xPos = Mathf.Max(_camera.WorldToScreenPoint(_chaser.transform.position).x / _canvas.scaleFactor + padding, 0);
        _rectTransform.anchoredPosition = new Vector2(xPos, _rectTransform.anchoredPosition.y);
        text.text = $"{(int)(_player.transform.position.x - _chaser.transform.position.x)}m";
    }
}
