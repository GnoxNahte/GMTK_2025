using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [field: SerializeField] public int CurrHealth { get; private set; }

    public Action OnDeath;

    public void SetMaxHealth(int health)
    {
        maxHealth = health;
        CurrHealth = health;
    }

    private void Start()
    {
        CurrHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        CurrHealth = Mathf.Clamp(CurrHealth - damage, 0, maxHealth);

        if (CurrHealth <= 0)
            OnDeath?.Invoke();
    }
}