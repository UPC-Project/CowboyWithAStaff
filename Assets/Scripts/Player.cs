using UnityEngine;

public class Player : HealthSystem
{
    public int healingPotions = 0;

    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private GameObject _facingPoint;

    [Header("Melee Attack")]
    [SerializeField] private int _meleeAttackDamage = 1;
    [SerializeField] private float _nextMeleeAttackTime;
    [SerializeField] private float _attackMeleeCooldown;
    [SerializeField] private float _hitRadius;

    [Header("Ranged Attack")]
    [SerializeField] private float _nextRangedAttackTime;
    [SerializeField] private float _attackRangedCooldown;

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
        if (_nextMeleeAttackTime > 0) return;

        _attackDirection = (_facingPoint.transform.position - transform.position).normalized;

        _animator.SetFloat("horizontal", _attackDirection.x);
        _animator.SetFloat("vertical", _attackDirection.y);
        _animator.SetBool("isAttacking", true);

        _playerMovement.canMove = false;

        _nextMeleeAttackTime = _attackMeleeCooldown;
    }

    // Triggered when X key is pressed
    public void OnRangedAttack()
    {
        if (_nextRangedAttackTime > 0) return;

        _attackDirection = (_facingPoint.transform.position - transform.position).normalized;

        _animator.SetFloat("horizontal", _attackDirection.x);
        _animator.SetFloat("vertical", _attackDirection.y);
        _animator.SetBool("isAttackingRanged", true);

        _playerMovement.canMove = false;


        _nextRangedAttackTime = _attackRangedCooldown;
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

    // Called to notifiy player movement when movement is valid
    public void EndAttackAnimation()
    {
        _animator.SetBool("isAttacking", false);
        _animator.SetBool("isAttackingRanged", false);
        _playerMovement.canMove = true;
    }

    public override void Death()
    {
        _animator.SetBool("isDead", true);
        _playerMovement.canMove = false;
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
