using UnityEngine;
using Constants;

public class Player : Health
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

    [Header("Block Skill")]
    [SerializeField] private float _nextBlockTime;
    [SerializeField] private float _attackBlockCooldown;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    private Vector2 _attackDirection;
    public bool _isDying = false;

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
        if (_nextBlockTime > 0)
        {
            _nextBlockTime -= Time.deltaTime;
        }
    }

    // ATTACK SYSTEM
    // Triggered when Z key or RMB is pressed
    public void OnMeleeAttack()
    {
        if (_nextMeleeAttackTime <= 0 && _playerMovement.canMove)
        {
            AttackAnimation(PlayerSkill.MeleeAttack.ToString());
            // MeleeAttack() is called in PlayerMeleeAttackStateBehaviour
        }
    }

    // Triggered when X key or LMB is pressed
    public void OnRangedAttack()
    {
        if (_nextRangedAttackTime <= 0 && _playerMovement.canMove)
        {
            AttackAnimation(PlayerSkill.RangedAttack.ToString());
            // RangedAttack() is called in PlayerRangedAttackStateBehaviour
        }
    }

    // Triggered when C key or MMB is pressed
    public void OnBlockSkill()
    {
        if (_nextBlockTime <= 0 && !_playerMovement.isAttacking)
        {
            // Change when animation is done
            AttackAnimation(PlayerSkill.Block.ToString());
            BlockSkill();
        }
    }

    private void BlockSkill()
    {
        Debug.Log("Block skill activated");
    }

    private void AttackAnimation(string attack)
    {
        _attackDirection = (_facingPoint.transform.position - transform.position).normalized;

        _animator.SetFloat("horizontal", _attackDirection.x);
        _animator.SetFloat("vertical", _attackDirection.y);
        // The trigger starts the animation and enters to the corresponding attack state
        _animator.SetTrigger(attack);

        _playerMovement.canMove = false;
    }

    // Called by PlayerRangedAttackStateBehaviour/PlayerMeleeAttackStateBehaviour when attack is completed
    public void OnExitAttackState(PlayerSkill playerSkill)
    {
        _playerMovement.canMove = true;
        // Cooldown begins once the animation has finished
        if (playerSkill == PlayerSkill.MeleeAttack)
        {
            _nextMeleeAttackTime = _attackMeleeCooldown;
        }
        else if (playerSkill == PlayerSkill.RangedAttack)
        {
            _nextRangedAttackTime = _attackRangedCooldown;
        } else
        {
            _nextBlockTime = _attackBlockCooldown;
        }
    }

    public void MeleeAttack()
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

    public void RangedAttack()
    {
        // The bullet damage is in the Bullet script
        BulletPool.Instance.RequestBullet(_facingPoint.transform.position, _facingPoint.transform.rotation);
    }

    public override void StartDeath()
    {
        if (!_isDying)
        {
            _animator.SetTrigger("Death");
            _playerMovement.canMove = false;
            _isDying = true;
        }
    }

    public override void Death()
    {
        GameState.Instance.Respawn();
        _isDying = false;
        _playerMovement.canMove = true;
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
            collision.gameObject.SetActive(false);
            GameState.Instance.RegisterCollectedItem(collision.gameObject);
        }
    }

}
