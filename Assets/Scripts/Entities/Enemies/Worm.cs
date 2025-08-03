using System;
using UnityEngine;
using VInspector;

public class Worm : EnemyBase
{
    [SerializeField] TriggerCollisionEvents groundChecker;
    [SerializeField] private float speed;
    
    [ShowInInspector] private bool _isMovingRight;

    private void Update()
    {
        transform.position += speed * Time.deltaTime * (_isMovingRight ? Vector3.right : Vector3.left);
    }

    private void OnEnable()
    {
        groundChecker.OnTriggerEnter += OnHitEnemy;
        groundChecker.OnTriggerExit += OnGroundCheckerExits;
    }

    private void OnDisable()
    {
        groundChecker.OnTriggerEnter -= OnHitEnemy;
        groundChecker.OnTriggerExit -= OnGroundCheckerExits;
    }

    private void OnHitEnemy(Collider2D other)
    {
        if (other.GetComponent<EnemyBase>())
            Flip();
    }
    
    private void OnGroundCheckerExits(Collider2D other)
    {
        if (other.GetComponent<PlatformBase>())
            Flip();
    }

    private void Flip()
    {
        _isMovingRight = !_isMovingRight;
        var scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
