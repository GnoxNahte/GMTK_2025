using System;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] private float invincibilityAlpha;
    
    private SpriteRenderer _sr;

    private PlayerMovement _movement;
    
    public void Init(PlayerMovement movement)
    {
        _movement = movement;
    }

    public void OnInvincibilityChange(bool toOn)
    {
        // throw new NotImplementedException();
    }
    
    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        bool ifFacingLeft = _movement.FacingDirection.x < 0;
        _sr.flipX = ifFacingLeft;
    }
}
