using UnityEngine;

public class MeleeEnemy : Enemy
{
    [SerializeField] private float _hitRadius;

    protected override void Attack()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(gameObject.transform.position, _hitRadius);
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
        Gizmos.DrawWireSphere(gameObject.transform.position, _hitRadius);
    }
}
