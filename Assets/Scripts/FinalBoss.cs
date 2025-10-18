using UnityEngine;

public class FinalBoss : RangedEnemy
{
    [SerializeField] private float _nextBrustAttackTime;
    [SerializeField] private int _attackTimeMultiplier = 1;
    [SerializeField] private int _timesAttacked = 0;
    [SerializeField] private bool _firstFase = true;

    protected override void Start()
    {
        base.Start();
        onAggro = true;
    }

    protected override void OnUpdate()
    {
        if (target)
        {
            if (_attackingTime <= 0f)
            {
                RotateTowardsTarget();
            }

            if (_nextAttackTime > 0)
            {
                _nextAttackTime -= Time.deltaTime;
                _attackingTime -= Time.deltaTime;
            }
            else
            {
                Attack();
                if (_timesAttacked == 2)
                {
                    _timesAttacked = 0;
                    _attackTimeMultiplier = 2;
                }
                else
                {
                    _timesAttacked++;
                    _attackTimeMultiplier = 1;
                }
                _nextAttackTime = _nextAttackRate * _attackTimeMultiplier;
                _attackingTime = _attackingRate;
            }

            // Check fase 
            if (health < health / 2)
            {
                _firstFase = false;
            }
        }
    }
}
