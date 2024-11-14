using UnityEngine;

public class Hittable : MonoBehaviour
{
    public int health = 100;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Add death effects, animations, and removal logic here
        Destroy(gameObject);
    }
}
