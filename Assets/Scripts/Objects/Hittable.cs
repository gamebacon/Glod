using UnityEngine;
using System;

public class Hittable : MonoBehaviour
{
    public int health = 0;
    private int maxHealth = 100;

    // Define a Die event that other scripts can subscribe to
    public event Action OnDie;
    public event Action OnDamage;

    private GameEntity gameEntity;

    void Start() {
        gameEntity = GetComponent<GameEntity>();
        health = maxHealth;
    }


    public void TakeDamage(int damage)
    {
        if (isDead())
        {
            return;
        }

        health -= damage;
        OnDamage?.Invoke();

        if (health <= 0)
        {
            Die();
        }

        gameEntity.entityCanvas.SetHealth(health, maxHealth);
    }

    public bool isDead()
    {
        return health <= 0;
    }

    void Die()
    {
        OnDie?.Invoke();
    }
}
