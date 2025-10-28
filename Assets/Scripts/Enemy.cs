using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public abstract class Enemy : HealthSystem
{
    [Header("References")]
    protected Rigidbody2D _rb;
    protected Animator _animator;

    [Header("Target")]
    public Transform target;
    [SerializeField] private float _aggroRadius;
    private bool _onAggro = false;

    [Header("Movement")]
    public float speed = 2f;
    public float rotateSpeed = 0.1f;
    public float distanceToStop = 1.5f;

    [Header("Attack")]
    [SerializeField] protected int _damage = 1;
    [SerializeField] protected float _nextAttackTime;
    [SerializeField] protected float _attackCooldown = 1f;

    // Animation helpers
    private Vector2 _lastDirection = Vector2.down;
    protected bool canMove = true;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {
        if (!target || !canMove) return;

        if (_nextAttackTime > 0f)
            _nextAttackTime -= Time.deltaTime;

        if (!_onAggro)
        {
            Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, _aggroRadius);
            foreach (Collider2D collider in objects)
            {
                if (collider.CompareTag("Player"))
                {
                    _onAggro = true;
                    break;
                }
            }
        }

        if (!_onAggro) return;

        RotateTowardsTarget();

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist <= distanceToStop)
        {
            _rb.linearVelocity = Vector2.zero;
            TryAttack();
        }
    }

    private void FixedUpdate()
    {
        if (!_onAggro || !canMove || !target)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist > distanceToStop)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            _rb.linearVelocity = dir * speed;
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
        }
    }

    protected virtual void RotateTowardsTarget()
    {
        Vector2 dir = (target.position - transform.position).normalized;

        Vector2 animDir = dir.sqrMagnitude > 0.01f ? dir : _lastDirection;
        if (dir.sqrMagnitude > 0.01f)
            _lastDirection = dir;

        _animator.SetFloat("horizontal", animDir.x);
        _animator.SetFloat("vertical", animDir.y);
        float normalized = Mathf.Clamp01(_rb.linearVelocity.magnitude / speed);
        _animator.SetFloat("speed", normalized);
    }


    private void TryAttack()
    {
        if (_nextAttackTime > 0f)
            return;

        canMove = false;
        _animator.SetBool("isAttacking", true);

        
    }

    public void EndAttackAnimation()
    {
        _animator.SetBool("isAttacking", false);
        canMove = true;

        // Cooldown begins once the animation has finished
        _nextAttackTime = _attackCooldown;
    }

    public override void Death()
    {
        canMove = false;
        _animator.SetBool("isDead", true);
        Destroy(gameObject);
    }

    protected abstract void Attack();

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _aggroRadius);
    }
}
