using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [field: SerializeField] public int MaxHealth{ get; private set; } = 100;
    [field: SerializeField] public int CurrHealth { get; private set; }

    public Action OnDeath;

    public void SetMaxHealth(int health)
    {
        MaxHealth = health;
        CurrHealth = health;
    }

    private void Start()
    {
        CurrHealth = MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        CurrHealth = Mathf.Clamp(CurrHealth - damage, 0, MaxHealth);

        if (CurrHealth <= 0)
            OnDeath?.Invoke();
    }
}