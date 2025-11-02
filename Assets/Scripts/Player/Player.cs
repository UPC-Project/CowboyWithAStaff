using UnityEngine;

public class Player : HealthSystem
{
    public int healingPotions = 0;
    [SerializeField] private PlayerMovement _playerMovement;

    [Header("Melee Attack")]
    [SerializeField] private int _meleeAttackDamage = 1;
    [SerializeField] private float _nextMeleeAttackTime;
    [SerializeField] private float _attackMeleeCooldown;
    [SerializeField] private float _hitRadius;

    [Header("Ranged Attack")]
    [SerializeField] private float _nextRangedAttackTime;
    [SerializeField] private float _attackRangedCooldown;
    [SerializeField] private GameObject _facingPoint;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    private Vector2 _attackDirection;


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
        if (_nextMeleeAttackTime <= 0 && !_playerMovement.isAttacking)
        {
            AttackAnimation("MeleeAttack");
            // MeleeAttack() is called in PlayerMeleeAttackStateBehaviour
        }
    }

    // Triggered when X key is pressed
    public void OnRangedAttack()
    {
        if (_nextRangedAttackTime <= 0 && !_playerMovement.isAttacking)
        {
            AttackAnimation("RangedAttack");
            // RangedAttack() is called in PlayerRangedAttackStateBehaviour
        }
    }

    private void AttackAnimation(string attack)
    {
        _attackDirection = (_facingPoint.transform.position - transform.position).normalized;

        _animator.SetFloat("horizontal", _attackDirection.x);
        _animator.SetFloat("vertical", _attackDirection.y);
        // The trigger starts the animation and enters to the corresponding attack state
        _animator.SetTrigger(attack); 

        _playerMovement.isAttacking = true;
    }

    // Called by PlayerRangedAttackStateBehaviour/PlayerMeleeAttackStateBehaviour when attack is completed
    public void OnExitAttackState(bool isMelee)
    {
        _playerMovement.isAttacking = false;
        // Cooldown begins once the animation has finished
        if (isMelee)
        {
            _nextMeleeAttackTime = _attackMeleeCooldown;
        }
        else
        {
            _nextRangedAttackTime = _attackRangedCooldown;
        }
    }

    public void MeleeAttack()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(gameObject.transform.position, _hitRadius);
        foreach (Collider2D collider in objects)
        {
            if (collider.CompareTag("Enemy"))
            {
                Debug.Log("found");
                collider.transform.GetComponent<Enemy>().TakeDamage(_meleeAttackDamage);
            }
        }
    }

    public void RangedAttack()
    {
        // The bullet damage is in the Bullet script
        GameObject bullet = BulletPool.Instance.RequestBullet(_facingPoint.transform.position, _facingPoint.transform.rotation);
    }

    public override void Death()
    {
        _animator.SetBool("isDead", true);
        Debug.Log("You died");
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
