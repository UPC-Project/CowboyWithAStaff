using UnityEngine;
using System.Collections;

public class RangedEnemy : Enemy
{
    public float distanceToShoot = 8f;
    public int bullets = 2;
    public float timeBetweenBullets = 0.1f;
    [SerializeField] protected Transform _firingPoint;
    [SerializeField] protected float _firingPointDistance;

    public override void Attack()
    {
        float h = _animator.GetFloat("horizontal");
        float v = _animator.GetFloat("vertical");
        Vector2 dir = new Vector2(h, v).normalized;
        if (dir == Vector2.zero) dir = Vector2.down;

        _firingPoint.localPosition = dir * 1.25f * _firingPointDistance;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _firingPoint.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);

        // Use _attackSounds later and modularize this in Enemy class
        AudioManager.Instance.Play("SnakeShoot");

        StartCoroutine(BurstAttack(_firingPoint.position, _firingPoint.rotation));
    }

    public IEnumerator BurstAttack(Vector3 fixedShootPosition, Quaternion fixedShootRotation)
    {
        // Ranged Enemy uses a different rotation too shoot the bullet, is not as lineal as the player
        // See preferences.
        for (int i = 0; i < bullets; i++)
        {
            BulletPool.Instance.RequestBullet(fixedShootPosition, fixedShootRotation);
            yield return new WaitForSeconds(timeBetweenBullets);
        }
    }

    protected override bool PlayerInRangeToAttack()
    {
        float distanceToTarget = Vector2.Distance(target.position, transform.position);
        return distanceToTarget <= distanceToShoot;
    }

    // Call it with SMB instead of Animation Event
    // Modularize this in Enemy class later
    public void PlayFootstepSound()
    {
        StartCoroutine(SoundUtils.PlayRandomSounds(_audioSourceWalk, _moveSounds, (1f, 5f), () => isWalking(), 0.5f));
    }
}

