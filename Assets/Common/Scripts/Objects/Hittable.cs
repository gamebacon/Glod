using UnityEngine;
using System;

public class Hittable : MonoBehaviour
{
    public float health = 0;
    private float maxHealth = 100;

    // Define a Die event that other scripts can subscribe to
    public event Action OnDie;
    public event Action OnDamage;

    private GameEntity gameEntity;

    void Start() {
        gameEntity = GetComponent<GameEntity>();
        maxHealth *= (int) gameObject.transform.localScale.x;
        health = maxHealth;


        // gameEntity.entityCanvas.SetHealth(health, maxHealth);
    }


    public void TakeDamage(float damage)
    {
        if (isDead())
        {
            return;
        }

        health = Math.Max(0, health - damage);
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
