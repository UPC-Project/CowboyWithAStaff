using UnityEngine;

public abstract class Enemy : HealthSystem
{
    public Transform target;
    [SerializeField] protected float _aggroRadius;
    [SerializeField] protected bool _onAggro = false;
    protected Rigidbody2D _rb;

    [Header("Animation")]
    [SerializeField] protected Animator _animator;
    protected Vector2 _lastDirection = Vector2.down;

    [Header("Movement")]
    public float speed;
    public float distanceToStop = 5f;

    [Header("Attack")]
    [SerializeField] protected int _damage;
    [SerializeField] protected float _nextAttackTime;
    [SerializeField] protected float _nextAttackRate;
    // The enemy is still attacking
    public bool isAttacking = false;

    protected virtual void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {
        if (target && _onAggro)
        {
            if (!isAttacking)
            {
                RotateTowardsTarget();
            }
            
            if (_nextAttackTime > 0)
            {
                _nextAttackTime -= Time.deltaTime;
            }
            else if (PlayerInRangeToAttack() && !isAttacking)
            {
                isAttacking = true;
                // Here the attack animation starts and calls AttackStateBehaviour.OnStateEnter() function
                _animator.SetTrigger("Attack"); 
            }
        }
        else if (!_onAggro)
        {
            Collider2D[] objects = Physics2D.OverlapCircleAll(gameObject.transform.position, _aggroRadius);
            foreach (Collider2D collider in objects)
            {
                if (collider.CompareTag("Player"))
                {
                    _onAggro = true;
                    break;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        OnFixedUpdate();
    }

    protected virtual void OnFixedUpdate()
    {
        if (!PlayerInRangeToStop() && _onAggro && !isAttacking)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            _rb.linearVelocity = dir * speed;
        }
        else
        {
            _rb.linearVelocity = new Vector2(0, 0);
        }
    }

    protected void RotateTowardsTarget()
    {
        Vector2 dir = (target.position - transform.position).normalized;

        Vector2 animDir = dir.sqrMagnitude > 0.01f ? dir : _lastDirection;
        if (dir.sqrMagnitude > 0.01f) _lastDirection = dir;

        _animator.SetFloat("horizontal", animDir.x);
        _animator.SetFloat("vertical", animDir.y);
        float normalized = Mathf.Clamp01(_rb.linearVelocity.magnitude / speed);
        _animator.SetFloat("speed", normalized);
    }

    // Called by AttackStateBehaviour when attack is started
    public abstract void Attack();

    // Called by AttackStateBehaviour when attack is completed
    public void OnExitAttackState()
    {
        isAttacking = false;
        // Cooldown begins once the animation has finished
        _nextAttackTime = _nextAttackRate;
    }

    protected virtual bool PlayerInRangeToStop()
    {
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        return distanceToTarget <= distanceToStop;
    }

    protected virtual bool PlayerInRangeToAttack()
    {
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        return distanceToTarget <= distanceToStop;
    }

    public override void Death()
    {
        _animator.SetBool("isDead", true);
    }


    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(gameObject.transform.position, _aggroRadius);
    }
}
