using System;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] private float invincibilityAlpha;
    [SerializeField] private SpriteRenderer sr;
    
    private Animator _animator;
    private PlayerMovement _movement;
    
    private static readonly int AnimId_InAir = Animator.StringToHash("IsInAir");
    private static readonly int AnimId_IsDead = Animator.StringToHash("IsDead");
    private static readonly int AnimId_xSpeed = Animator.StringToHash("xSpeed");
    
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
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        bool ifFacingLeft = _movement.FacingDirection.x < 0;
        sr.flipX = ifFacingLeft;
        
        _animator.SetBool(AnimId_InAir, _movement.IsInAir);
        _animator.SetBool(AnimId_IsDead, _movement.IsDead);
        
        _animator.SetFloat(AnimId_xSpeed, Mathf.Abs(_movement.Velocity.x));
    }
}
