using UnityEngine;

public class MeleeEnemy : Enemy
{
    [SerializeField] private float _hitRadius = 1f;

    protected override void Attack()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, _hitRadius);
        foreach (Collider2D collider in objects)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<Player>().TakeDamage(_damage);
            }
        }
    }

    // Called by animation event when the animation is in the hit frame
    public void OnAttackHit()
    {
        Attack();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _hitRadius);
    }
}
