using UnityEngine;

public class MeleeEnemy : Enemy
{
    [SerializeField] private float _hitRadius;

    public override void Attack()
    {
        // Use _attackSounds later and modularize this in Enemy class
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
        // Use _damageSounds and moudlarize this en Enemy class later
        AudioManager.Instance.Play("ZombieTakeDamage");
        base.TakeDamage(damage); 
    }

    // Modularize this in Enemy class later
    public void PlayFootstepSound()
    {
        StartCoroutine(SoundUtils.PlayRandomSounds(_audioSourceWalk, _moveSounds, (1f, 5f), () => isWalking(), 0.5f));
    }
}