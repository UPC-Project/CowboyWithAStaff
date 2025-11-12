using UnityEngine;
using System.Collections;

public class FinalBoss : RangedEnemy
{
    [SerializeField] private int _attackTimeMultiplier = 1;
    [SerializeField] private int _timesAttacked = 0;
    [SerializeField] private int _restartLoopAt = 2;
    [SerializeField] private bool _firstFase = true;

    [SerializeField] private float _hitRadius;
    [SerializeField] private bool _isRetreating = false;

    public float safeDistance;
    public float distanceToStopMelee = 2f;
    public float speedSecondFase = 5f;
    public float retreatSpeed;

    // Parameters wont be necessary when animation is added
    public float _attackingTime;
    public float _attackingRate;

    [SerializeField] private WinGameUI _winGameUI;

    protected override void OnUpdate()
    {
        if (target)
        {
            if (_attackingTime <= 0)
            {
                OldRotateTowardsTarget();
            }

            if (_nextAttackTime > 0)
            {
                _nextAttackTime -= Time.deltaTime;
                _attackingTime -= Time.deltaTime;
            }
            // Boss starts with ranged attacks, ends up attacking in melee
            else if (_firstFase && PlayerInRangeToAttack())
            {
                RangedAttack();

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
                _attackingTime = _attackingRate;
            }
            else if (!_firstFase && !_isRetreating)
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

                MeleeAttack();
            }
        }

        // Check fase 
        if (health <= (maxHealth / 2) && _firstFase)
        {
            _firstFase = false;

            // Change everything necessary for second fase
            distanceToStop = distanceToStopMelee;
            speed = speedSecondFase;
            _nextAttackRate = 0;
            _nextAttackTime = 0;
            _timesAttacked = 0;
            _attackingTime = 0;
            _attackingRate = 0;
        }
    }

    protected override void OnFixedUpdate()
    {
        if (!PlayerInRangeToStop() && !_isRetreating && _attackingTime <= 0)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            _rb.linearVelocity = dir * speed;
        }
        else
        {
            _rb.linearVelocity = new Vector2(0, 0);
        }
    }

    private void RangedAttack()
    {
        // Default Attack from RangedEnemy class
        Attack();
    }

    private void MeleeAttack()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, _hitRadius);
        foreach (Collider2D collider in objects)
        {
            if (collider.CompareTag("Player"))
            {
                collider.transform.GetComponent<Player>().TakeDamage(_damage);
                // See if it's worth it
                //StartCoroutine(_playerMovement.Stune());
                StartCoroutine(Retreat());
            }
            // Boss destroys bullets when atacking melee, will be implementing depending on the animation
            // Will be implemented in SMB probably
            //if (collider.CompareTag("Bullet"))
            //{
            //    (collider.gameObject).SetActive(false);
            //}
        }
    }

    private IEnumerator Retreat()
    {
        _isRetreating = true;

        while (Vector2.Distance(transform.position, target.position) < safeDistance)
        {
            Vector2 dir = (transform.position - target.position).normalized;
            transform.position += (Vector3)dir * retreatSpeed * Time.deltaTime;

            yield return null;
        }
        // After 3 attacks, the boss will wait 3 second before restart
        if (_timesAttacked == _restartLoopAt)
        {
            yield return new WaitForSeconds(3f);
        }

        _isRetreating = false;
    }

    // Melee attack range
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _hitRadius);
    }
    // Delete this when animation is implemented, use default rotation from Enemy class
    private void OldRotateTowardsTarget()
    {
        Vector2 targetDirection = target.position - transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;
        Quaternion q = Quaternion.Euler(new Vector3(0, 0, angle));
        float difference = Quaternion.Angle(transform.localRotation, q);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, q, 0.1f);
    }

    // Delete this when animation is implemented, use default rotation from RangedEnemy class 
    public override void Attack()
    {
        StartCoroutine(BurstAttack(_firingPoint.position, transform.rotation));
    }

    // It's not repealed
    public override void RepelFromPLayer(Vector3 playerPos, float repelForce) { }

    // override not necessary when animation is implemented
    public override void StartDeath()
    {
        canMove = false;
        // this call will be in Death()
        StartCoroutine(_winGameUI.WinGame());
    }
    public override void Death()
    {
        //StartCoroutine(WinGame());
    }
}


