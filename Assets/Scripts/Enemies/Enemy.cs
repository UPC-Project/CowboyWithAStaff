using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Health
{
    public Transform target;
    protected Rigidbody2D _rb;
    public Collider2D col2D;
    public SpriteRenderer spriteRenderer;
    [SerializeField] protected float _aggroRadius;
    [SerializeField] protected bool _onAggro = false;
    private Vector3 _startPosition;
    private bool _respawnFlag = true;

    [Header("Animation")]
    [SerializeField] protected Animator _animator;
    protected Vector2 _lastDirection = Vector2.down;

    [Header("Sound")]
    [SerializeField] protected AudioSource _audioSource;
    [SerializeField] protected List<AudioClip> _idleSounds;
    [SerializeField] protected List<AudioClip> _moveSounds;
    [SerializeField] protected List<AudioClip> _attackSounds;
    [SerializeField] protected List<AudioClip> _damageSounds;
    [SerializeField] protected float _nextIdleSoundTime;
    [SerializeField] protected float _idleSoundRateTime;

    [Header("Movement")]
    public float speed;
    public float distanceToStop = 5f;
    public bool _isRepealing = false;
    public bool _isStuned = false;

    [Header("Attack")]
    [SerializeField] protected int _damage;
    [SerializeField] protected float _nextAttackTime;
    [SerializeField] protected float _nextAttackRate;
    // The enemy is not attacking nor defeated
    public bool canMove = true;

    protected virtual void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
        col2D = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _startPosition = transform.position;
        StartCoroutine(SoundUtils.PlayRandomSounds(_audioSource, _idleSounds, (2f, 8f), () => canMove, 0.2f));
    }

    private void Update()
    {
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {
        if (target && _onAggro)
        {
            if (canMove)
            {
                RotateTowardsTarget();
            }

            if (_nextAttackTime > 0)
            {
                _nextAttackTime -= Time.deltaTime;
            }
            else if (PlayerInRangeToAttack() && canMove)
            {
                canMove = false;
                // Here the attack animation starts and calls AttackStateBehaviour.OnStateEnter() function
                _animator.SetTrigger("Attack");
            }
        }
        else if (!_onAggro)
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
        if (!PlayerInRangeToStop() && _onAggro && canMove && !_isRepealing)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            _rb.linearVelocity = dir * speed;
        }
        else if (!_isRepealing)
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
        canMove = true;
        // Cooldown begins once the animation has finished
        _nextAttackTime = _nextAttackRate;
        StartCoroutine(SoundUtils.PlayRandomSounds(_audioSource, _idleSounds, (2f, 8f), () => canMove, 0.2f));
    }

    // Called by EnemyDeathSMB when attack is completed
    public override void Death()
    {
        gameObject.SetActive(false);
        _onAggro = false;
    }

    public void ResetEnemyState()
    {
        gameObject.SetActive(true);
        StartCoroutine(JustRespawn());
        ResetHealth();
        transform.position = _startPosition;
        _nextAttackTime = 0;
        _onAggro = false;
        canMove = true;
        var tempColor = spriteRenderer.color;
        tempColor.a = 1;
        spriteRenderer.color = tempColor;
        col2D.enabled = true;

        _rb.linearVelocity = Vector2.zero;

        // Reset animator
        _animator.SetBool("isDead", false);
        _animator.SetFloat("horizontal", 0f);
        _animator.SetFloat("vertical", -1f);
        _animator.SetFloat("speed", 0f);
        _animator.Play("idle", 0, 0f);

        // Sounds
        StartCoroutine(SoundUtils.PlayRandomSounds(_audioSource, _idleSounds, (2f, 8f), () => canMove, 0.2f));
    }

    public virtual void RepelFromPLayer(Vector3 playerPos, float repelForce)
    {
        StartCoroutine(RepealSelf());
        Vector2 dir = -((playerPos - transform.position).normalized);
        _rb.linearVelocity = dir * repelForce;
    }

    // UTILS
    // Avoids activating aggro at respawn
    public override void StartDeath()
    {
        GameState.Instance.RegisterActivatedEnemy(this.gameObject);
        _animator.SetBool("isDead", true);
        canMove = false;
        col2D.enabled = false;
    }
    IEnumerator JustRespawn()
    {
        _respawnFlag = false;
        yield return new WaitForSeconds(.2f);
        _respawnFlag = true;
    }

    IEnumerator RepealSelf()
    {
        // Repealing enemy
        _isRepealing = true;
        yield return new WaitForSeconds(.2f);
        _isRepealing = false;
        // Enemy stuned
        canMove = false;
        _animator.SetFloat("speed", 0f);
        yield return new WaitForSeconds(.5f);
        canMove = true;
    }

    public void ChangeAlpha()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    IEnumerator FadeOutCoroutine()
    {
        Color color = spriteRenderer.color;
        float startAlpha = color.a;
        float elapsed = 0f;

        while (elapsed < 1.2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 1.2f;
            color.a = Mathf.Lerp(startAlpha, 0f, t);
            spriteRenderer.color = color;
            yield return null;
        }

        color.a = 0f;
        spriteRenderer.color = color;
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

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(gameObject.transform.position, _aggroRadius);
    }
}
