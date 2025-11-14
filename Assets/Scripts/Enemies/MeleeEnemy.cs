using UnityEngine;

public class MeleeEnemy : Enemy
{
    [SerializeField] private float _hitRadius;

    private bool _wasIdleLastFrame;
    [SerializeField] private AudioSource _idleAudioSource;

    protected override void Start()
    {
        base.Start();
        _wasIdleLastFrame = true;
        _idleAudioSource.Play();
    }

    public override void Update()
    {
        base.Update();
        ZombieIdle();
    }
    public override void Attack()
    {
        AudioManager.Instance.Play("ZombieHit");
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, _hitRadius);
        foreach (Collider2D collider in objects)
        {
            if (collider.CompareTag("Player"))
            {
                collider.transform.GetComponent<Player>().TakeDamage(_damage);
            }
        }
    }

    // Melee attack range
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _hitRadius);
    }

    public override void TakeDamage(int damage)
    {
        AudioManager.Instance.Play("ZombieTakeDamage");
        base.TakeDamage(damage); 
    }

    public void ZombieIdle()
    {

        float h = _animator.GetFloat("horizontal");
        float v = _animator.GetFloat("vertical");
        bool isCurrentlyMoving = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;

        Debug.Log($"h: {h}, v: {v}");
        bool isCurrentlyIdle = !isCurrentlyMoving;

        if (isCurrentlyIdle == _wasIdleLastFrame)
        {
            return;
        }

        if (isCurrentlyIdle)
        {
            _idleAudioSource.Play();
        }
        else
        {
            _idleAudioSource.Stop();
        }

        _wasIdleLastFrame = isCurrentlyIdle;
    }

    public void PlayFootstepSound()
    {
        AudioManager.Instance.Play("ZombieWalk");
    }
}