using UnityEngine;
using System;

public class Hittable : MonoBehaviour
{
    public int health = 100;

    // Define a Die event that other scripts can subscribe to
    public event Action OnDie;
    public event Action OnDamage;

    public void TakeDamage(int damage)
    {
        health -= damage;
        OnDamage?.Invoke();
        if (health <= 0)
        {
            Debug.Log(health);
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Death");
        // Trigger the Die event if there are any subscribers
        OnDie?.Invoke();

    }
}
