using UnityEngine;
using System.Collections;

public class RangedEnemy : Enemy
{
    public float distanceToShoot = 8f;
    public int bullets = 2;
    public float timeBetweenBullets = 0.1f;
    [SerializeField] private GameObject _firingPoint;

    protected override void Start()
    {
        base.Start();
        _attackingRate= bullets * timeBetweenBullets;
        _attackingTime = _attackingRate;
    }

    protected override void OnFixedUpdate()
    {
        if(Vector2.Distance(target.position, transform.position) >= distanceToStop && _onAggro && _attackingTime <= 0)
        {
            _rb.linearVelocity = transform.up * speed;
        }
        else
        {
            _rb.linearVelocity = new Vector2(0, 0);
        }
    }

    protected override void Attack()
    {
        if (Vector2.Distance(target.position, transform.position) <= distanceToShoot)
        {
            // Ranged Enemy uses a different rotation too shoot the bullet, is not as lineal as the player
            // See preferences.
            GameObject bullet = BulletPool.Instance.RequestBullet(_firingPoint.transform.position, transform.rotation, "Enemy");
            StartCoroutine(BurstAttack(_firingPoint.transform.position));
        }
    }

    public IEnumerator BurstAttack(Vector3 fixedShootPosition)
    {
        // Ranged Enemy uses a different rotation too shoot the bullet, is not as lineal as the player
        // See preferences.
        for (int i = 0; i < bullets; i++)
        {
            BulletPool.Instance.RequestBullet(fixedShootPosition, transform.rotation, "Enemy");
            yield return new WaitForSeconds(timeBetweenBullets);
        }
    }
}
