using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float speed;
    private ObjectPool _pool;

    private Transform _target;
    private CircleCollider2D _collider;
    private Vector3 _direction;

    private void Awake()
    {
        _collider = GetComponent<CircleCollider2D>();
    }

    public void Init(Transform target, ObjectPool pool, Vector2 dir)
    {
        _target = target;
        _pool = pool;
        _direction = dir;
    }

    private void Update()
    {
        if (_target)
        {
            transform.position = Vector3.MoveTowards(transform.position, _target.position, speed * Time.deltaTime);
            if ((transform.position - _target.position).sqrMagnitude < _collider.radius * _collider.radius)
            {
                OnHit(_target.gameObject);
            }
        }
        else
        {
            transform.position += speed * Time.deltaTime * _direction;
        }
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
