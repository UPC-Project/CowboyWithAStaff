using UnityEngine;

public abstract class HealthSystem : MonoBehaviour
{
    [Header("Health")]
    public int health;
    public int maxHealth = 5;

    public virtual void Awake()
    {
        health = maxHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Death();
    }

    // if necessary, override this method.
    public virtual void Death()
    {
        Destroy(gameObject);
    }

}
