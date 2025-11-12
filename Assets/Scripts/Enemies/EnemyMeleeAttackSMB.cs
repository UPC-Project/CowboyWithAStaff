using UnityEngine;

public class EnemyMeleeAttackSMB : StateMachineBehaviour
{
    private bool _hasAttacked;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _hasAttacked = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_hasAttacked && stateInfo.normalizedTime >= 0.57f)
        {
            var enemy = animator.GetComponentInParent<MeleeEnemy>();
            enemy?.Attack();
            _hasAttacked = true;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var enemy = animator.GetComponentInParent<MeleeEnemy>();
        enemy?.OnExitAttackState();
    }
}
