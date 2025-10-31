using UnityEngine;

public class MeleeEnemy : Enemy
{
    [SerializeField] private float _hitRadius;

    public override void Attack()
    {
        //Debug.Log($"enter Attack | obj: {gameObject.name} | compID: {this.GetInstanceID()} | time: {Time.time}");
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, _hitRadius);
        foreach (Collider2D collider in objects)
        {
            //Debug.Log(collider.name + " | Tag: " + collider.tag + " | Layer: " + LayerMask.LayerToName(collider.gameObject.layer));
            if (collider.CompareTag("Player"))
            {
                //Debug.Log("found");
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
