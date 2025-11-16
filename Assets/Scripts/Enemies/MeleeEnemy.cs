using UnityEngine;

public class MeleeEnemy : Enemy
{
    [SerializeField] private float _hitRadius;

    public override void Attack()
    {
        // Use _attackSounds later and modularize this in Enemy class
        SoundUtils.PlayARandomSound(_audioSource, _attackSounds);
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

    

    // Call it with SMB instead of Animation Event
    
   
}