using UnityEngine;

public class MeleeEnemy : Enemy
{
    [SerializeField] private float _hitRadius;

    public override void Attack()
    {
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
}
