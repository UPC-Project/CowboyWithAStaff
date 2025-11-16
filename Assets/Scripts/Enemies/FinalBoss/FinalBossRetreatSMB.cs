using Constants;
using UnityEngine;

public class FinalBossRetreatSMB : StateMachineBehaviour
{
    private FinalBoss _finalBoss;

    // Change to update if needed
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _finalBoss = animator.GetComponentInParent<FinalBoss>();
        _finalBoss.invulnerable = true;
        if (stateInfo.IsName("retreat"))
        {
            _finalBoss?.ReproduceRetreatSounds(FinalBossAnimationStates.retreat);
        }
        else if (stateInfo.IsName("retreat_spin"))
        {
            _finalBoss?.ReproduceRetreatSounds(FinalBossAnimationStates.retreatSpin);
        }
        else
        {
            _finalBoss?.ReproduceRetreatSounds(FinalBossAnimationStates.retreatEnd);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("retreat_end")) _finalBoss.invulnerable = false;
    }
}
