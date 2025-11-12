using UnityEngine;

public class EnemyRangedAttackSMB : StateMachineBehaviour
{
    private bool _hasAttacked;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
            _hasAttacked = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_hasAttacked && stateInfo.normalizedTime >= 0.29f)
        {
            var enemy = animator.GetComponentInParent<RangedEnemy>();
            enemy?.Attack();
            _hasAttacked = true;
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var enemy = animator.GetComponentInParent<RangedEnemy>();
        enemy?.OnExitAttackState();
    }
}
