using System;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Player _player;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Init(Player player)
    {
        _player = player;
        
    }

    private void Update()
    {
        _rectTransform.anchorMax = new Vector2((float)_player.Health.CurrHealth / _player.Health.MaxHealth, 1);
    }
}
