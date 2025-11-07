using System.Collections;
using UnityEngine;

public abstract class Enemy : HealthSystem
{
    public Transform target;
    protected Rigidbody2D _rb;
    [SerializeField] protected bool _onAggro = false;
    [SerializeField] protected float _aggroRadius;
    private Vector3 _startPosition;
    private bool _respawnFlag = true;

    [Header("Movement")]
    public float speed;
    public float rotateSpeed = 0.05f;
    public float distanceToStop = 5f;

    [Header("Attack")]
    [SerializeField] protected int _damage;
    [SerializeField] protected float _nextAttackTime;
    [SerializeField] protected float _nextAttackRate;
    // The enemy is still attacking
    protected float _attackingTime = 0f;
    protected float _attackingRate;

    protected virtual void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
        _startPosition = transform.position;
        _attackingTime = _attackingRate;
    }

    private void Update()
    {
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {
        if (target && _onAggro)
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
            else
            {
                Attack();
                _nextAttackTime = _nextAttackRate;
                _attackingTime = _attackingRate;
            }
        }
        else
        {
            Collider2D[] objects = Physics2D.OverlapCircleAll(gameObject.transform.position, _aggroRadius);
            foreach (Collider2D collider in objects)
            {
                if (collider.CompareTag("Player") && _respawnFlag)
                {
                    _onAggro = true;
                    GameState.Instance.RegisterActivatedEnemy(this.gameObject);
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
        if (Vector2.Distance(target.position, transform.position) >= distanceToStop && _onAggro && _attackingTime <= 0)
        {
            _rb.linearVelocity = transform.up * speed;
        }
        else
        {
            _rb.linearVelocity = new Vector2(0, 0);
        }
    }

    protected void RotateTowardsTarget()
    {
        Vector2 targetDirection = target.position - transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;
        Quaternion q = Quaternion.Euler(new Vector3(0, 0, angle));
        float difference = Quaternion.Angle(transform.localRotation, q);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, q, rotateSpeed);
    }

    protected abstract void Attack();

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(gameObject.transform.position, _aggroRadius);
    }

    public override void Death()
    {
        gameObject.SetActive(false);
        _onAggro = false;
    }

    public void ResetEnemyState()
    {
        StartCoroutine(JustRespawn());
        ResetHealth();
        transform.position = _startPosition;
        _nextAttackTime = 0;
        _onAggro = false;
        _rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(true);
    }

    // Avoids activating aggro at respawn
    IEnumerator JustRespawn()
    {
        _respawnFlag = false;
        yield return new WaitForSeconds(.1f);
        _respawnFlag = true;
    }
}
