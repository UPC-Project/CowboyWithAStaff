using UnityEngine;
using System;

public class Player : HealthSystem
{
    public event Action OnPlayerDied;
    public int healingPotions = 0;

    [Header("Melee Attack")]
    [SerializeField] private int _meleeAttackDamage = 1;
    [SerializeField] private float _nextMeleeAttackTime;
    [SerializeField] private float _attackMeleeCooldown;
    [SerializeField] private float _hitRadius;

    [Header("Ranged Attack")]
    [SerializeField] private float _nextRangedAttackTime;
    [SerializeField] private float _attackRangedCooldown;
    [SerializeField] private GameObject _facingPoint;



    private void Update()
    {
        if (_nextMeleeAttackTime > 0)
        {
            _nextMeleeAttackTime -= Time.deltaTime;
        }
        if (_nextRangedAttackTime > 0)
        {
            _nextRangedAttackTime -= Time.deltaTime;
        }
    }

    // ATTACK SYSTEM
    // Triggered when Z key is pressed
    public void OnMeleeAttack()
    {
        if (_nextMeleeAttackTime <= 0)
        {
            MeleeAttack();
            _nextMeleeAttackTime = _attackMeleeCooldown;
        }
    }

    // Triggered when X key is pressed
    public void OnRangedAttack()
    {
        if (_nextRangedAttackTime <= 0)
        {
            RangedAttack();
            _nextRangedAttackTime = _attackRangedCooldown;
        }
    }

    private void MeleeAttack()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(gameObject.transform.position, _hitRadius);
        foreach (Collider2D collider in objects)
        {
            if (collider.CompareTag("Enemy"))
            {
                collider.transform.GetComponent<Enemy>().TakeDamage(_meleeAttackDamage);
            }
        }
    }

    private void RangedAttack()
    {
        // The bullet damage is in the Bullet script
        GameObject bullet = BulletPool.Instance.RequestBullet(_facingPoint.transform.position, _facingPoint.transform.rotation);
    }

    public override void Death()
    {
        OnPlayerDied?.Invoke();
    }


    // Comment this function if you don't want to see the melee range attack
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gameObject.transform.position, _hitRadius);
    }

    // HEAL SYSTEM
    // Triggered when H key is pressed
    public void OnHeal()
    {
        if (healingPotions > 0)
        {
            health = maxHealth;
            healingPotions -= 1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("HealingPotion"))
        {
            healingPotions += 1;
            Destroy(collision.gameObject);
        }
    }

}
