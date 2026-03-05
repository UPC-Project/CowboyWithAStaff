using Constants;
using UnityEngine;

public class FinalBossMeleeAttackSMB : StateMachineBehaviour
{
    private bool _hasAttacked;
    private FinalBoss _finalBoss;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _hasAttacked = false;
        _finalBoss = animator.GetComponentInParent<FinalBoss>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_hasAttacked && stateInfo.normalizedTime >= 0.25f)
        {
            _finalBoss?.MeleeAttack();
            _hasAttacked = true;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _finalBoss?.OnExitAttackState(FinalBossAnimationStates.meleeAtack);
    }
}
