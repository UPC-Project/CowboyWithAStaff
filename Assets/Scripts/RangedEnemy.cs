using UnityEngine;

public class RangedEnemy : Enemy
{
    public float distanceToShoot = 8f;
    [SerializeField] private Transform _firingPoint;

    protected override void Attack()
    {
        if (Vector2.Distance(target.position, transform.position) <= distanceToShoot)
        {
            float h = _animator.GetFloat("horizontal");
            float v = _animator.GetFloat("vertical");
            Vector2 dir = new Vector2(h, v).normalized;
            if (dir == Vector2.zero) dir = Vector2.down; // fallback


            _firingPoint.localPosition = dir * 1.25f;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            _firingPoint.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);

            GameObject bullet = BulletPool.Instance.RequestBullet(_firingPoint.position, _firingPoint.rotation);
        }

    }

}
