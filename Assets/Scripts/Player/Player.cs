using UnityEngine;
using Constants;

public class Player : Health
{
    public int healingPotions = 0;
    [SerializeField] private PlayerMovement _playerMovement;
    public bool _inBossFight = false;

    [Header("Melee Attack")]
    [SerializeField] private int _meleeAttackDamage = 1;
    [SerializeField] private float _nextMeleeAttackTime;
    [SerializeField] private float _attackMeleeCooldown;
    public float hitRadius;

    [Header("Ranged Attack")]
    [SerializeField] private float _nextRangedAttackTime;
    [SerializeField] private float _attackRangedCooldown;
    [SerializeField] private GameObject _facingPoint;

    [Header("Block Skill")]
    [SerializeField] private float _nextBlockTime;
    [SerializeField] private float _attackBlockCooldown;
    public float repelForce;
    public bool invulnerable = false;

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

    // ATTACK
    // Triggered when Q key or LMB is pressed
    public void OnMeleeAttack()
    {
        if (_nextMeleeAttackTime <= 0 && _playerMovement.canMove)
        {
            AttackAnimation(PlayerSkill.MeleeAttack.ToString());
            // MeleeAttack() is called in PlayerMeleeAttackStateBehaviour
        }
    }

    // Triggered when E key or RMB is pressed
    public void OnRangedAttack()
    {
        if (_nextRangedAttackTime <= 0 && _playerMovement.canMove)
        {
            AttackAnimation(PlayerSkill.RangedAttack.ToString());
            // RangedAttack() is called in PlayerRangedAttackStateBehaviour
        }
    }

    // Triggered when Space key or MMB is pressed
    public void OnBlockSkill()
    {
        if (_nextBlockTime <= 0 && _playerMovement.canMove)
        {
            // Change when animation is done
            AttackAnimation(PlayerSkill.Block.ToString());
            invulnerable = true;
        }
    }

    public void BlockSkill()
    {
            if (!_inBossFight) RepelEnemies();
    }



    private void RepelEnemies()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, hitRadius * 2);
        foreach (Collider2D collider in objects)
        {
            if (collider.CompareTag("Enemy"))
            {
                collider.transform.GetComponent<Enemy>().RepelFromPLayer(transform.position, repelForce);
            }
        }
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
            invulnerable = false;
            _nextBlockTime = _attackBlockCooldown;
        }
    }

    public void MeleeAttack()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(gameObject.transform.position, hitRadius);
        foreach (Collider2D collider in objects)
        {
            if (collider.CompareTag("Enemy"))
            {
                collider.transform.GetComponent<Enemy>().TakeDamage(_meleeAttackDamage);
            }
        }

        AudioManager.Instance.Play("PlayerMeleeAttack");
    }

    public void RangedAttack()
    {
        // The bullet damage is in the Bullet script
        BulletPool.Instance.RequestBullet(_facingPoint.transform.position, _facingPoint.transform.rotation);
        AudioManager.Instance.Play("PlayerShoot");
    }

    // DEATH
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

    // HEALTH
    // Triggered when H key is pressed
    public void OnHeal()
    {
        if (healingPotions > 0)
        {
            AudioManager.Instance.Play("PotionUse");
            health = maxHealth;
            healingPotions -= 1;
        }
    }

    public override void TakeDamage(int damage)
    {
        if (!invulnerable) health -= damage;
        if (health <= 0) StartDeath();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("HealingPotion"))
        {
            AudioManager.Instance.Play("PotionPickup");
            healingPotions += 1;
            collision.gameObject.SetActive(false);
            GameState.Instance.RegisterCollectedItem(collision.gameObject);
        }
    }


    public void PlayFootstepSound()
    {
        AudioManager.Instance.Play("PlayerWalk");
    }
    // Comment this function if you don't want to see the melee range attack
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gameObject.transform.position, hitRadius);
    }

}
