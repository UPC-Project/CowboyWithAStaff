using Constants;
using UnityEngine;

public class FinalBossRangedAttackSMB : StateMachineBehaviour
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
        if (!_hasAttacked && stateInfo.normalizedTime >= 0.5f)
        {
            _finalBoss?.RangedAttack();
            _hasAttacked = true;
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _finalBoss?.OnExitAttackState(FinalBossAnimationStates.rangedAttack);
    }
}
