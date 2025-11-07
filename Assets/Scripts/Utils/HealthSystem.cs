using UnityEngine;

public abstract class HealthSystem : MonoBehaviour
{
    [Header("Health")]
    public int health;
    public int maxHealth = 5;

    private void Awake()
    {
        health = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) StartDeath();
    }

    // Reset healt state for respawn at checkpoint
    public void ResetHealth()
    {
        health = maxHealth;
    }


    public abstract void StartDeath();

    // if necessary, override this method.
    public virtual void Death()
    {
        Destroy(gameObject);
    }

}
