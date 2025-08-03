using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float speed;
    private ObjectPool _pool;

    private CircleCollider2D _collider;
    private Vector3 _direction;

    private void Awake()
    {
        _collider = GetComponent<CircleCollider2D>();
    }

    public void Init(ObjectPool pool, Vector2 dir)
    {
        if (dir == Vector2.zero)
            dir = Vector2.right;

        _pool = pool;
        _direction = dir;
    }

    private void Update()
    {
        transform.position += speed * Time.deltaTime * _direction;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnHit(other.gameObject);
    }

    private void OnHit(GameObject other)
    {
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy)
        {
            enemy.TakeDamage(damage, enemy.transform.position);
        }
        
        _pool.Release(gameObject);
    }
}
