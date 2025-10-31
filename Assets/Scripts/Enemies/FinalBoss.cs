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

    public float _attackingTime;
    public float _attackingRate;
    protected override void OnUpdate()
    {
        if (target)
        {
            if (_attackingTime <= 0f)
            {
                RotateTowardsTarget();
            }

            if (_nextAttackTime > 0)
            {
                _nextAttackTime -= Time.deltaTime;
                _attackingTime -= Time.deltaTime;
            }
            // Boss starts with ranged attacks, ends up attacking in melee
            else if (_firstFase)
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
        if (health < (maxHealth / 2) && _firstFase)
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
        if (Vector2.Distance(target.position, transform.position) >= distanceToStop && !_isRetreating && _attackingTime <= 0)
        {
            _rb.linearVelocity = transform.up * speed;
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
                StartCoroutine(Retreat());
            }
            // Boss destroys bullets when atacking melee, will be implementing depending on the animation
            //if (collider.CompareTag("Bullet") && ...)
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
}