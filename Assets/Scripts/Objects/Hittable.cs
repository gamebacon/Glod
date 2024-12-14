using UnityEngine;
using System;
using UnityEditor.SceneManagement;

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
        health -= damage;
        OnDamage?.Invoke();
        if (health <= 0)
        {
            Debug.Log(health);
            Die();
        }

        gameEntity.entityCanvas.SetHealth(health, maxHealth);
    }

    void Die()
    {
        OnDie?.Invoke();
        Destroy(this);
    }
}
