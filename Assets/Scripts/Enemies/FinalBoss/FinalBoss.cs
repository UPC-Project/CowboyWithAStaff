using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss : RangedEnemy
{
    [SerializeField] private int _attackTimeMultiplier = 1;
    [SerializeField] private int _timesAttacked = 0;
    [SerializeField] private int _restartLoopAt = 2;
    [SerializeField] private bool _firstPhase = true;

    [SerializeField] private float _hitRadius;
    [SerializeField] private bool _isRetreating = false;
    private float _distanceToAttack;
    public bool invulnerable = false; // Invulneabilty handled in TransitionSMB

    public float safeDistance;
    public float distanceToStopMelee = 2f;
    public float speedSecondPhase = 5f;
    public float retreatSpeed;
    private float _retreatTime;
    private float _retreatTimeCooldown = 4f;

    [SerializeField] private WinGameUI _winGameUI;

    // Extra sounds
    [Header("Extra Boss Sound")]
    [SerializeField] protected List<AudioClip> _meleeAttackSounds;
    [SerializeField] protected List<AudioClip> _retreatSounds;

    protected override void Start()
    {
        base.Start();
        _distanceToAttack = distanceToShoot;
        _retreatTime = _retreatTimeCooldown;
    }

    protected override void OnUpdate()
    {
        if (target)
        {
            if (canMove)
            {
                RotateTowardsTarget();
            }

            if (_nextAttackTime > 0)
            {
                _nextAttackTime -= Time.deltaTime;
            }
            // Boss starts with ranged attacks, ends up attacking in melee
            else if (_firstPhase && PlayerInRangeToAttack() && canMove)
            {
                Debug.Log("Final Boss Ranged Attack");
                canMove = false;
                _animator.SetTrigger("Attack"); // Ranged Attack called

                // There is a pattern when attacking
                // The attack sequence is: (attack) - 2sec - (attack) - 2sec - (attack) - 4sec
                if (_timesAttacked == _restartLoopAt)
                {
                    _timesAttacked = 0;
                    _attackTimeMultiplier = 2;
                }
                else
                {
                    _timesAttacked++;
                    _attackTimeMultiplier = 1;
                }
                _nextAttackTime = _nextAttackRate * _attackTimeMultiplier;
            }
            else if (!_firstPhase && !_isRetreating && canMove && PlayerInRangeToAttack())
            {
                // There is a pattern when attacking
                if (_timesAttacked == _restartLoopAt)
                {
                    _timesAttacked = 0;
                }
                else
                {
                    _timesAttacked++;
                }
                canMove = false;
                _animator.SetTrigger("Attack"); // Melee Attack called
            }
        }
    }

    protected override void OnFixedUpdate()
    {
        if (!PlayerInRangeToStop() && !_isRetreating && canMove)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            _rb.linearVelocity = dir * speed;
        }
        else
        {
            _rb.linearVelocity = new Vector2(0, 0);
        }
    }

    public void RangedAttack()
    {
        // Default Attack from RangedEnemy class
        Attack();
    }

    public void MeleeAttack()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, _hitRadius);
        foreach (Collider2D collider in objects)
        {
            if (collider.CompareTag("Player"))
            {
                collider.transform.GetComponent<Player>().TakeDamage(_damage);
                SoundUtils.PlayARandomSound(_audioSource, _meleeAttackSounds);
                // See if it's worth it
                //StartCoroutine(_playerMovement.Stune());

            }
            // Boss destroys bullets when atacking melee, will be implementing depending on the animation
            // Will be implemented in SMB probably
            //if (collider.CompareTag("Bullet"))
            //{
            //    (collider.gameObject).SetActive(false);
            //}
        }
    }

    public void OnExitAttackState(FinalBossAnimationStates state)
    {
        Debug.Log("Final Boss Exit Attack State");
        canMove = true;
        StartCoroutine(SoundUtils.PlayRandomSoundsLoop(_audioSource, _idleSounds, (2f, 10f), () => canMove));
        if (state == FinalBossAnimationStates.rangedAttack)
        {
            _nextAttackTime = _nextAttackRate;
        }
        else if (!_animator.GetBool("isDead"))
        {
            StartCoroutine(Retreat());
            // see what to do after Melee attack 
        }
    }

    private IEnumerator Retreat()
    {
        _isRetreating = true;
        _animator.SetBool("isRetreating", true);

        while (Vector2.Distance(transform.position, target.position) < safeDistance && _retreatTime > 0f)
        {
            Vector2 dir = (transform.position - target.position).normalized;
            transform.position += (Vector3)dir * retreatSpeed * Time.deltaTime;
            _retreatTime -= Time.deltaTime;

            yield return null;
        }
        _retreatTime = _retreatTimeCooldown;
        _animator.SetBool("isRetreating", false);

        // After 3 attacks, the boss will wait 3 second before restart
        if (_timesAttacked == _restartLoopAt)
        {
            yield return new WaitForSeconds(3f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }
        _isRetreating = false;
    }

    public override void TakeDamage(int damage)
    {
        if (!invulnerable) base.TakeDamage(damage);
        // Check phase 
        if (health <= (maxHealth / 2) && _firstPhase)
        {
            _firstPhase = false;
            _animator.SetTrigger("secondPhase");

            // Change everything necessary for second phase
            _distanceToStop = distanceToStopMelee;
            _distanceToAttack = _distanceToStop;
            speed = speedSecondPhase;
            _nextAttackRate = 0;
            _nextAttackTime = 0;
            _timesAttacked = 0;
        }
    }

    // It's not repealed
    public override void RepelFromPLayer(Vector3 playerPos, float repelForce) { }

    public override void Death()
    {
        StartCoroutine(_winGameUI.WinGame());
    }

    // SOUNDS
    public void ReproduceRetreatSounds(FinalBossAnimationStates state)
    {
        // if (state == FinalBossAnimationStates.retreat) then blabla
    }

    // UTILS
    protected override bool PlayerInRangeToAttack()
    {
        float distanceToTarget = Vector2.Distance(target.position, transform.position);
        return distanceToTarget <= _distanceToAttack;
    }

    // Melee attack range
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _hitRadius);
    }
}


